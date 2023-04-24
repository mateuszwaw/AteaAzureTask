using Azure;
using Azure.Data.Tables;

namespace AteaAzureTask.Shared.Entities
{
    public class FunctionCallLogEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public bool? IsSuccess { get; set; }
    }
}
