using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using NReco.VideoConverter;
using NReco.VideoInfo;

namespace TeamStreamFunctions
{
    public class VideoProcessor
    {
        public static async Task Run(Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"***** Video Processor : {name} *****");
            
            string blobVideoPath = Keys.baseUrl + "videos/" + name;
            string ffmpegToolPath = Environment.GetEnvironmentVariable("HOME") + "\\site\\wwwroot\\App_Data";

            var ffProbe = new FFProbe();
            ffProbe.ToolPath = ffmpegToolPath;

            var videoInfo = ffProbe.GetMediaInfo(blobVideoPath);
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

                var converter = new FFMpegConverter();
                converter.FFMpegToolPath = ffmpegToolPath;

                if (frameLocation == 0)
                {
                    converter.GetVideoThumbnail(blobVideoPath, ms);
                }
                else
                {
                    converter.GetVideoThumbnail(blobVideoPath, ms, frameLocation);
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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Keys.storageConnectionString);

            // Create a blob client for interacting with the blob service.
            blobClient = storageAccount.CreateCloudBlobClient();

            blobThumbContainer = blobClient.GetContainerReference(Keys.thumbnailContainer);

            CloudBlockBlob blob = blobThumbContainer.GetBlockBlobReference(Utils.CreateBlobWithImageExtension(blobFileName, totalFrames, frame));
            blob.Properties.ContentType = "image/jpg";
            await blob.UploadFromStreamAsync(ms);
        }

        

    }
}