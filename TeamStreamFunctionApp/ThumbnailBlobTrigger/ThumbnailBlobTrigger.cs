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

namespace TeamStreamFunctionApp
{
    public class ThumbnailBlobTrigger
    {


        // Retrieve the desired database id (name) from the configuration file
        private static readonly string databaseId = "Videos";
        // Retrieve the desired collection id (name) from the configuration file
        private static readonly string collectionId = "VideoCollection";
        // Retrieve the DocumentDB URI from the configuration file
        private static readonly string endpointUrl = "https://teamstream.documents.azure.com:443/";
        // Retrieve the DocumentDB Authorization Key from the configuration file
        private static readonly string authorizationKey = "";

        private static readonly string cogservAPIKey = "";


        public static async Task Run(Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"Thumbnail Blob trigger function processing: {myBlob}");

            string videoId = GetGuidFromBlobName(name);

            //Call API
            string result = await CallVisionAPIAnalyze(myBlob);
            VisionAnalysis data = JsonConvert.DeserializeObject<VisionAnalysis>(result);
            log.Info($"Data received from API");

            //Get Doc DB
            DocumentClient client;
            client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            var database = client.CreateDatabaseQuery().Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = databaseId });
            }
            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == collectionId).ToArray().FirstOrDefault();
            // If the previous call didn't return a Collection, it is necessary to create it
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(database.SelfLink, new DocumentCollection { Id = collectionId });
            }
            //get the document to update
            dynamic videoDocument = client.CreateDocumentQuery<Document>(collection.SelfLink).Where(
                d => d.Id == videoId).AsEnumerable().FirstOrDefault();
            log.Info("done with DB...for now");

            if (videoDocument != null)
            {
                Video videoToUpdate = videoDocument;
                
                videoToUpdate.VisionAPI.Add(data);               

                var savedVideoDocument = await client.ReplaceDocumentAsync(videoDocument, videoToUpdate);

                log.Info("Replaced. Now im done with DB.");
            }

        }

        static async Task<string> CallVisionAPIAnalyze(Stream image)
        {
            using (var client = new HttpClient())
            {
                var content = new StreamContent(image);
                var url = "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description&language=en";
                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogservAPIKey);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var httpResponse = await client.PostAsync(url, content);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return await httpResponse.Content.ReadAsStringAsync();
                }
            }
            return null;
        }

        static string GetGuidFromBlobName(string filename)
        {
            return filename.Substring(filename.IndexOf('_') + 1, 36);
        }

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
            public Description description { get; set; }
            public string requestId { get; set; }
            public Metadata metadata { get; set; }            
            public string id { get; set; }
        }

        public class Video
        {
            public Guid Id { get; set; }
            public string VideoId { get; set; }
            public string VideoName { get; set; }
            public string VideoUrl { get; set; }
            public string VideoKeywords { get; set; }
            public string VideoThumbnail { get; set; }
            public DateTime VideoDate { get; set; }
            public List<VisionAnalysis> VisionAPI { get; set; }
        }

        //public class Description
        //{
        //    public List<string> tags { get; set; }
        //    public List<Caption> captions { get; set; }
        //}

        //public class Caption
        //{
        //    public string text { get; set; }
        //    public double confidence { get; set; }
        //}

        //public class Metadata
        //{
        //    public int width { get; set; }
        //    public int height { get; set; }
        //    public string format { get; set; }
        //}
    }
}