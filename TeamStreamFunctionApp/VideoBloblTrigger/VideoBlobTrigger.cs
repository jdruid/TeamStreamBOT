using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using NReco.VideoConverter;
using NReco.VideoInfo;
using TeamStreamFunctionApp.Shared;

namespace TeamStreamFunctionApp
{
    public class VideoBlobTrigger
    {
        
        public static async Task Run(Stream myBlob, string name, TraceWriter log)
        {
            string videoPath = Keys.baseUrl + "videos/" + name;

            var ffProbe = new NReco.VideoInfo.FFProbe();
            var videoInfo = ffProbe.GetMediaInfo(videoPath);

            //video.Duration - 00:00:19.2416670
            TimeSpan duration = videoInfo.Duration;
            log.Info($"Video Info: duration: {videoInfo.Duration}");

            //need to figure out the following
            /*
             * 0-60sec = every 10 sec
             * 60-120sec = every 20 sec
             * 120+ every 30
             */
            int totalFrames = 0;
            int chunkSize = 0;

            if ((duration.TotalSeconds > 0) || (duration.TotalSeconds < 60))
            {
                totalFrames = Convert.ToInt32(Math.Round(duration.TotalSeconds / 6, 0));
                chunkSize = 6;
            }
            else if ((duration.TotalSeconds > 60) || (duration.TotalSeconds < 120))
            {
                totalFrames = Convert.ToInt32(Math.Round(duration.TotalSeconds / 12, 0));
                chunkSize = 12;
            }
            else
            {
                totalFrames = Convert.ToInt32(Math.Round(duration.TotalSeconds / 24, 0));
                chunkSize = 24;
            }

            log.Info($"Video Info: chunk Size: {chunkSize} frames: {totalFrames}");

            int frameLocation = 0;
            int i;

            //debuggin only
            //totalFrames = 1;

            for (i = 0; i <= totalFrames; i++)
            {
                MemoryStream ms = new MemoryStream();

                var converter = new NReco.VideoConverter.FFMpegConverter();
                if (frameLocation == 0)
                {
                    converter.GetVideoThumbnail(videoPath, ms);
                }
                else
                {
                    converter.GetVideoThumbnail(videoPath, ms, frameLocation);
                }

                ms.Position = 0;

                //move to blob
                await MoveThumbnailToStorage(i, totalFrames, name, ms);

                frameLocation += chunkSize;
            }

            log.Info($"Processed {i} images");

            log.Info($"Video Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

        }

        static async Task MoveThumbnailToStorage(int frame, int totalFrames, string blobFileName, MemoryStream ms)
        {
            CloudBlobClient blobClient;
            CloudBlobContainer blobThumbContainer;

            string blobThumbContainerName = "thumbnails";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Keys.storageConnectionString);

            // Create a blob client for interacting with the blob service.
            blobClient = storageAccount.CreateCloudBlobClient();

            blobThumbContainer = blobClient.GetContainerReference(blobThumbContainerName);

            CloudBlockBlob blob = blobThumbContainer.GetBlockBlobReference(CreateBlobWithImageExtension(blobFileName, totalFrames, frame));
            blob.Properties.ContentType = "image/jpg";
            await blob.UploadFromStreamAsync(ms);
        }

        static string CreateBlobWithImageExtension(string filename, int totalFrames, int frame)
        {
            return filename.Replace(".mp4", $"_{ (object)totalFrames}_{ (object)frame}.jpg");
        }


    }
}
