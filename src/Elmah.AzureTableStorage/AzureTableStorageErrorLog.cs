using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;
using System.Collections;
using System.Linq;

namespace Elmah.AzureTableStorage
{
    /// <summary>
    /// An <see cref="ErrorLog"/> implementation that uses Microsoft Azure 
    /// Storage as its backing store.
    /// </summary>

    public class AzureTableStorageErrorLog : ErrorLog
    {
        private readonly TableServiceContext _tableContext;
        private readonly CloudTable _cloudTable;
        private const string TableName = "Elmah";

        private const int MaxAppNameLength = 60;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableStorageErrorLog"/> class
        /// using a dictionary of configured settings.
        /// </summary>

        public AzureTableStorageErrorLog(IDictionary config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            var connectionString = ElmahHelper.GetConnectionString(config);

            //
            // If there is no connection string to use then throw an 
            // exception to abort construction.
            //

            if (connectionString.Length == 0)
                throw new ApplicationException("Connection string is missing for the Azure Table Storage error log.");

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            _tableContext = tableClient.GetTableServiceContext();
            _cloudTable = tableClient.GetTableReference(TableName);
            _cloudTable.CreateIfNotExists();

            //
            // Set the application name as this implementation provides
            // per-application isolation over a single store.
            //

            var appName = config.Find("applicationName", string.Empty);

            if (appName.Length > MaxAppNameLength)
            {
                throw new ApplicationException(string.Format(
                    "Application name is too long. Maximum length allowed is {0} characters.",
                    MaxAppNameLength.ToString("N0")));
            }

            ApplicationName = appName;
        }

        /// <summary>
        /// Gets the name of this error log implementation.
        /// </summary>

        public override string Name
        {
            get { return "Microsoft Azure Storage Error Log"; }
        }

        /// <summary>
        /// Logs an error to the database.
        /// </summary>
        /// <remarks>
        /// Use the stored procedure called by this implementation to set a
        /// policy on how long errors are kept in the log. The default
        /// implementation stores all errors for an indefinite time.
        /// </remarks>

        public override string Log(Error error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            var elmahEntity = new ElmahEntity
            {
                AllXml = ErrorXml.EncodeString(error),
                ApplicationName = error.ApplicationName,
                HostName = error.HostName,
                Message = error.Message,
                Source = error.Source,
                StatusCode = error.StatusCode,
                Type = error.Type,
                User = error.User,
            };

            var tableOperation = TableOperation.Insert(elmahEntity);
            _cloudTable.Execute(tableOperation);

            return elmahEntity.RowKey;
        }

        /// <summary>
        /// Returns a page of errors from the database in descending order 
        /// of logged time.
        /// </summary>

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex", pageIndex, null);
            if (pageSize < 0) throw new ArgumentOutOfRangeException("pageSize", pageSize, null);

            // Skip is not allowed, so we will take extra records and then discard ones that weren't requested.
            // This obviously has a performance hit, but since users are usually looking at the latest ones, this may be OK for most scenarios.
            var errorEntities = _tableContext
                .CreateQuery<ElmahEntity>(TableName)
                .AsTableServiceQuery(_tableContext)
                .Take((pageIndex + 1) * pageSize)
                .ToList()
                .Skip(pageIndex * pageSize);

            foreach (var errorEntity in errorEntities)
            {
                var error = ErrorXml.DecodeString(errorEntity.AllXml);
                errorEntryList.Add(new ErrorLogEntry(this, errorEntity.RowKey, error));
            }

            // Azure Table Storage cannot return the total number of records,
            // so if the max number of errors are displayed,
            // we will report an extra element to indicate to the user that more records may exist
            return errorEntryList.Count == pageSize 
                ? (pageIndex + 1) * pageSize + 1 
                : pageIndex * pageSize + errorEntryList.Count;
        }

        /// <summary>
        /// Returns the specified error from the database, or null 
        /// if it does not exist.
        /// </summary>

        public override ErrorLogEntry GetError(string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (id.Length == 0) throw new ArgumentException(null, "id"); 
            
            var elmahEntity = _cloudTable.CreateQuery<ElmahEntity>()
                .Where(e => e.RowKey == id)
                .ToList()
                .First();

            var error = ErrorXml.DecodeString(elmahEntity.AllXml);
            return new ErrorLogEntry(this, id, error);
        }
    }
}
