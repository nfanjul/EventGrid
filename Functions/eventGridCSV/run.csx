#r "Microsoft.Azure.WebJobs.Extensions.EventGrid"
#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Host.Bindings.Runtime;
using Microsoft.WindowsAzure.Storage; 
using Microsoft.WindowsAzure.Storage.Blob;

static string storageAccountConnectionString = System.Environment.GetEnvironmentVariable("myblobstorage_STORAGE");
static string thumbContainerOK = System.Environment.GetEnvironmentVariable("myContainerOK");
static string thumbContainerKO = System.Environment.GetEnvironmentVariable("myContainerKO");

public static async Task Run(EventGridEvent myEvent, Stream inputBlob, TraceWriter log)
{  
    using(MemoryStream myStream = new MemoryStream())
    {
        long count = 0;
        using (var reader = new StreamReader(inputBlob))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                count++;
            }
        }

        CloudBlockBlob blockBlob;
        if(count >= 5)
        {
            blockBlob = GetStorage(thumbContainerOK, myEvent);
        }
        else
        {
            blockBlob = GetStorage(thumbContainerKO, myEvent);
        }

        await blockBlob.UploadFromStreamAsync(myStream);
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

private static CloudBlockBlob GetStorage(string containerName, EventGridEvent myEvent)
{
    CloudBlobClient blobClient = GetClient();
    CloudBlobContainer container = blobClient.GetContainerReference(containerName);
    string blobName = GetBlobNameFromUrl((string)myEvent.Data["url"]);
    CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
    return blockBlob;
}

private static string GetBlobNameFromUrl(string bloblUrl)
{
    var myUri = new Uri(bloblUrl);
    var myCloudBlob = new CloudBlob(myUri);
    return myCloudBlob.Name;
}
