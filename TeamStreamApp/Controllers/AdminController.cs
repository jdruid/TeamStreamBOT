using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using TeamStreamApp.Models;
using System.Configuration;

using TeamStreamApp.Repository;

namespace TeamStreamApp.Controllers
{
    public class AdminController : Controller
    {
        private VideoRepository _videoRespository;

        static CloudBlobClient blobClient;
        static CloudBlobContainer blobThumbContainer;
        static CloudBlobContainer blobVideoContainer;

        const string blobVideoContainerName = "videos";
        const string blobThumbContainerName = "thumbnails";
        private static readonly string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        public AdminController()
        {
            _videoRespository = new VideoRepository();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Videos()
        {
            return View();
        }

        // GET: Admin
        public async Task<ActionResult> Manage()
        {

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Create a blob client for interacting with the blob service.
                blobClient = storageAccount.CreateCloudBlobClient();

                blobThumbContainer = blobClient.GetContainerReference(blobThumbContainerName);
                await blobThumbContainer.CreateIfNotExistsAsync();
                await blobThumbContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                blobVideoContainer = blobClient.GetContainerReference(blobVideoContainerName);                
                await blobVideoContainer.CreateIfNotExistsAsync();
                await blobVideoContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });


                // Gets all Cloud Block Blobs in the blobContainerName and passes them to teh view
                List<Uri> allBlobs = new List<Uri>();
                foreach (IListBlobItem blob in blobVideoContainer.ListBlobs())
                {
                    if (blob.GetType() == typeof(CloudBlockBlob))
                        allBlobs.Add(blob.Uri);
                }

                return View(allBlobs);
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }

        }
        

        /// <summary> 
        /// Task<ActionResult> UploadAsync() 
        /// Documentation References:  
        /// - UploadFromFileAsync Method: https://msdn.microsoft.com/en-us/library/azure/microsoft.windowsazure.storage.blob.cloudpageblob.uploadfromfileasync.aspx
        /// </summary> 
        [HttpPost]
        public async Task<ActionResult> UploadAsync(Video video)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a blob client for interacting with the blob service.
            blobClient = storageAccount.CreateCloudBlobClient();
            
            blobVideoContainer = blobClient.GetContainerReference(blobVideoContainerName);
            await blobVideoContainer.CreateIfNotExistsAsync();
            await blobVideoContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            
            try
            {
                string blobFileName = string.Empty;
                var client = new HttpClient();

                HttpFileCollectionBase files = Request.Files;
                int fileCount = files.Count;

                if (fileCount > 0)
                {
                    for (int i = 0; i < fileCount; i++)
                    {
                        blobFileName = GetRandomBlobName(files[i].FileName);

                        //upload to Azure Blob Storage VIDEOS
                        CloudBlockBlob blob = blobVideoContainer.GetBlockBlobReference(blobFileName);
                        await blob.UploadFromStreamAsync(files[i].InputStream);

                        if (ModelState.IsValid)
                        {
                            video.Id = Guid.Parse(GetGuidFromBlobName(blobFileName));
                            video.RawUrl = baseUrl + "videos/" + blobFileName;
                            _videoRespository.InsertVideo(video);
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }
       
        /// <summary> 
        /// Task<ActionResult> DeleteImage(string name) 
        /// Documentation References:  
        /// - Delete Blobs: https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/#delete-blobs
        /// </summary> 
        [HttpPost]
        public async Task<ActionResult> DeleteImage(string name)
        {
            try
            {
                Uri uri = new Uri(name);
                string filename = Path.GetFileName(uri.LocalPath);

                var blob = blobThumbContainer.GetBlockBlobReference(filename);
                await blob.DeleteIfExistsAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        /// <summary> 
        /// Task<ActionResult> DeleteAll(string name) 
        /// Documentation References:  
        /// - Delete Blobs: https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/#delete-blobs
        /// </summary> 
        [HttpPost]
        public async Task<ActionResult> DeleteAll()
        {
            try
            {

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Create a blob client for interacting with the blob service.
                blobClient = storageAccount.CreateCloudBlobClient();

                blobThumbContainer = blobClient.GetContainerReference(blobThumbContainerName);
                await blobThumbContainer.CreateIfNotExistsAsync();
                await blobThumbContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                blobVideoContainer = blobClient.GetContainerReference(blobVideoContainerName);
                await blobVideoContainer.CreateIfNotExistsAsync();
                await blobVideoContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                foreach (var blob in blobVideoContainer.ListBlobs())
                {
                    if (blob.GetType() == typeof(CloudBlockBlob))
                    {
                        await ((CloudBlockBlob)blob).DeleteIfExistsAsync();
                    }
                }

                foreach (var blob in blobThumbContainer.ListBlobs())
                {
                    if (blob.GetType() == typeof(CloudBlockBlob))
                    {
                        await ((CloudBlockBlob)blob).DeleteIfExistsAsync();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        
        private string BytesToSrcString(byte[] bytes) => "data:image/png;base64," + Convert.ToBase64String(bytes);


        /// <summary> 
        /// string GetRandomBlobName(string filename): Generates a unique random file name to be uploaded  
        /// </summary> 
        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }

        private string GetGuidFromBlobName(string filename)
        {
            return filename.Substring(filename.IndexOf('_') + 1, 36);
        }


    }

}
