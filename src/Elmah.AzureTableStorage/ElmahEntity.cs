using System.Data.Services.Common;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Elmah.AzureTableStorage
{
    [DataServiceKey("PartitionKey", "RowKey")]
    internal class ElmahEntity : TableEntity
    {
        public ElmahEntity()
            : base(string.Empty, (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"))
        {
        }

        public string ApplicationName { get; set; }
        public string HostName { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
        public int StatusCode { get; set; }
        public string AllXml { get; set; }
    }
}
