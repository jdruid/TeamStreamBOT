using System;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Configuration;
using System.Text;
using TeamStreamFunctionApp.Shared;

namespace TeamStreamFunctionApp
{
    public class ThumbnailBlobTrigger
    {
       

        public static async Task Run(Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"Thumbnail Blob trigger function processing: {myBlob}");

            if (myBlob.Length == 0)
                return;

            //https://teamstreamstorage.blob.core.windows.net/thumbnails/1636300269168499863_ebe1a4bb-3985-4d21-932c-a37c1b30f0a7_1_1.jpg
            string videoId = GetGuidFromBlobName(name);

            //Call API
            string result = await CallVisionAPIAnalyze(myBlob);
            VisionAnalysis data = JsonConvert.DeserializeObject<VisionAnalysis>(result);
            data.thumbnailUrl = Keys.baseUrl + "thumbnails/" + name;
            data.thumbnailIndex = GetThumbnailIndex(name);
            data.thumbnailCount = GetThumbnailCount(name);
            data.id = Guid.NewGuid();
            data.videoId = Guid.Parse(videoId);
            log.Info($"Data received from API");

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            //Console.WriteLine(json);
            //log.Info(json);

            //call vision save
            await CallVisionAPISave(data);
            log.Info($"Data sent to save");

        }

        static async Task<string> CallVisionAPIAnalyze(Stream image)
        {
            using (var client = new HttpClient())
            {
                var content = new StreamContent(image);
                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Keys.cogservAPIKey);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var httpResponse = await client.PostAsync(Keys.visionApiURL, content);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return await httpResponse.Content.ReadAsStringAsync();
                }
            }
            return null;
        }

        static async Task<string> CallVisionAPISave(VisionAnalysis content)
        {
            using (var client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(content);
                var requestData = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Keys.videoApiURL, requestData);
                var result = await response.Content.ReadAsStringAsync();

                return result;
            }            
        }
        
        static string GetGuidFromBlobName(string filename)
        {
            return filename.Substring(filename.IndexOf('_') + 1, 36);
        }

        static int GetThumbnailIndex(string filename)
        {
           int lastunderscore = filename.LastIndexOf('_') + 1; //58 or 59
            int period = filename.IndexOf('.'); //61
            int diff = period - lastunderscore;

            return Convert.ToInt32(filename.Substring(lastunderscore, diff));
        }

        static int GetThumbnailCount(string filename)
        {
            int middleunderscore = filename.IndexOf('_') + 38;
            int lastnunderscore = filename.LastIndexOf('_');
            int diff = lastnunderscore - middleunderscore;

            return Convert.ToInt32(filename.Substring(middleunderscore, diff));
        }

        //Classes
        public class Caption
        {
            public string text { get; set; }
            public double confidence { get; set; }
        }

        public class Description
        {
            public List<string> tags { get; set; }
            public List<Caption> captions { get; set; }
        }

        public class Metadata
        {
            public int width { get; set; }
            public int height { get; set; }
            public string format { get; set; }
        }

        public class VisionAnalysis
        {
            public Guid id { get; set; }
            public Guid videoId { get; set; }
            public string requestId { get; set; }
            public string thumbnailUrl { get; set; }
            public int thumbnailIndex { get; set; }
            public int thumbnailCount { get; set; }
            public Description description { get; set; }
            public Metadata metadata { get; set; }
        }


    }
}