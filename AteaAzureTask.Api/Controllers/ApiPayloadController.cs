using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AteaAzureTask.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiPayloadController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ApiPayloadController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<string> Get(string id)
        {
            var connectionString = _config.GetValue<string>("ConnectionStrings:AzureAccountStorage");
            var blobContainerName = _config.GetValue<string>("AzureAccountStorage:BlobContainerName");

            var blobClient = new BlobClient(connectionString, blobContainerName, id);

            using var streamReader = new MemoryStream();
            await blobClient.DownloadToAsync(streamReader);

            var content = Encoding.UTF8.GetString(streamReader.ToArray());

            return content;
        }
    }
}
