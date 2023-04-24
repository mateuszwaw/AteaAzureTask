using AteaAzureTask.Shared.Entities;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace AteaAzureTask.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLogsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ApiLogsController(IConfiguration config) 
        {
            _config = config;
        }

        [HttpGet]
        public async Task<IEnumerable<FunctionCallLogEntity>> Get(DateTime dateFrom, DateTime dateTo)
        {
            var connectionString = _config.GetValue<string>("ConnectionStrings:AzureAccountStorage");
            var tableName = _config.GetValue<string>("AzureAccountStorage:TableName");

            var tableClient = new TableClient(connectionString, tableName);

            var dateF = new DateTimeOffset(dateFrom.ToUniversalTime());
            var dateT = new DateTimeOffset(dateTo.ToUniversalTime());

            var tableLogs =  tableClient.QueryAsync<FunctionCallLogEntity>(x => x.Timestamp >= dateF && x.Timestamp <= dateT);

            var results = new List<FunctionCallLogEntity>();

            await foreach(var result in tableLogs)
            {
                results.Add(result);
            }

            return results;
        }
    }
}
