using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AteaAzureTask.Shared.Entities;
using Microsoft.Extensions.Configuration;

namespace AteaAzureTask
{
    public class ApiCallFunction
    {
        private static IConfiguration _configuration;

        static ApiCallFunction()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
        }

        [FunctionName("ApiCallFunction")]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer)
        {
            var timestamp = DateTime.UtcNow;
            var guid = Guid.NewGuid().ToString();

            var url = _configuration.GetValue<string>("Values:APIUrl");
            var connectionString = _configuration.GetValue<string>("Values:ConnectionString");
            var azureTableName = _configuration.GetValue<string>("Values:AzureTableName");
            var azureBlobContainer = _configuration.GetValue<string>("Values:AzureBlobContainerName");

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);

            var jsonString = await response.Content.ReadAsStringAsync();

            var tableClient = new TableClient(connectionString, azureTableName);
            var blobClient = new BlobClient(connectionString, azureBlobContainer, guid);

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);

            await blobClient.UploadAsync(new MemoryStream(byteArray), true);
            await tableClient.AddEntityAsync(new FunctionCallLogEntity()
            {
                PartitionKey = "apiCallUnit",
                RowKey = guid,
                Timestamp = timestamp,
                IsSuccess = response.IsSuccessStatusCode
            });

            client.Dispose();
        }
    }
}
