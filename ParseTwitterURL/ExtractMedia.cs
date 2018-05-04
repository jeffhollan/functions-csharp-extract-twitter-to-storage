
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ParseTwitterURL
{
    public static class ExtractMedia
    {
        [FunctionName("Extract_Media_to_Storage")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequest req,
            TraceWriter log)
        {
            log.Info("C# trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var body = JObject.Parse(requestBody);

            JArray mediaUrls = (JArray)body["MediaUrls"];

            if(mediaUrls.Count > 0)
            {
                foreach(string url in mediaUrls)
                {
                    var response = await client.GetAsync(url);
                    var blob = blobContainer.GetBlockBlobReference(url.Substring(url.LastIndexOf('/') + 1));
                    await blob.UploadFromStreamAsync(await response.Content.ReadAsStreamAsync());
                }
            }
            return new OkResult();
        }

        private static HttpClient client = new HttpClient();
        private static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        private static CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        private static CloudBlobContainer blobContainer = blobClient.GetContainerReference("media");
    }
}
