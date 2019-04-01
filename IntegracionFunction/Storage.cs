// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IntegracionFunction
{
    public static class Storage
    {
        static readonly string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=myeventgridfunction;AccountKey=ekyfREQdK7fOw3bdNN/S0F/oBMesqZnHXL1BNiexHHSc1+ozlXu1mvEmQYjx+p5wcZSKLWt9Anm1zQcILww==;EndpointSuffix=core.windows.net";
        static readonly string containerOK = Environment.GetEnvironmentVariable("myContainerOK");
        static readonly string containerKO = Environment.GetEnvironmentVariable("myContainerKO");
        private static ILogger _log;
        private static CloudBlobClient _blobClient;

        [FunctionName("Storage")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            _log = log;
            log.LogInformation(eventGridEvent.Data.ToString());
            try
            {
                var data = eventGridEvent.Data as JObject;
                var url = data.ToObject<StorageBlobCreatedEventData>().Url;
                _blobClient = GetClient();
                var blobName = GetBlobNameFromUrl(url);
                await ReadAzureFile(blobName, url, eventGridEvent);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.InnerException.Message);
                log.LogError(ex, "***********************ERROR");
            }
        }

        private static async Task ReadAzureFile(string file, string url,EventGridEvent eventGridEvent)
        {
            long count = 0;

            CloudBlobContainer container = _blobClient.GetContainerReference("files");
            CloudBlob blob = container.GetBlobReference(file);
            using (var stream = blob.OpenReadAsync().Result)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        count++;
                    }
                }
                CloudBlockBlob blockBlob;
                if (count >= 5)
                {
                    blockBlob = GetStorage(containerOK, url, eventGridEvent);
                }
                else
                {
                    blockBlob = GetStorage(containerKO, url, eventGridEvent);
                }
                await blockBlob.UploadFromStreamAsync(stream);
            }
        }

        private static CloudBlobClient GetClient()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient;
        }

        private static CloudBlockBlob GetStorage(string containerName, string url ,EventGridEvent myEvent)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            var blobName = GetBlobNameFromUrl(url);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            return blockBlob;
        }

        private static string GetBlobNameFromUrl(string bloblUrl)
        {
            var myUri = new Uri(bloblUrl);
            var myCloudBlob = new CloudBlob(myUri);
            return myCloudBlob.Name;
        }
    }
}
