using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using TeamStreamFunctions.Models;

namespace TeamStreamFunctions
{
    public class ImageProcessor
    {
        public static async Task Run(Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"***** Thumbnail Processor : {name} *****");

            if (myBlob.Length == 0)
                return;

            //https://teamstreamstorage.blob.core.windows.net/thumbnails/1636300269168499863_ebe1a4bb-3985-4d21-932c-a37c1b30f0a7_1_1.jpg
            string videoId = Utils.GetGuidFromBlobName(name);

            //Call API
            string result = await CallVisionAPIAnalyze(myBlob);
            VisionAnalysis data = JsonConvert.DeserializeObject<VisionAnalysis>(result);
            data.thumbnailUrl = Keys.baseUrl + "thumbnails/" + name;
            data.thumbnailIndex = Utils.GetThumbnailIndex(name);
            data.thumbnailCount = Utils.GetThumbnailCount(name);
            data.id = Guid.NewGuid();
            data.videoId = Guid.Parse(videoId);
            log.Info($"Data received from API");

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            
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

        
    }
}