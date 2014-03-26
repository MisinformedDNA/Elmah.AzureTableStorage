using Microsoft.WindowsAzure;
using System;
using System.Collections;
using System.Configuration;

namespace Elmah.AzureTableStorage
{
    /// <summary>
    /// Includes methods that are internal to Elmah, but are useful. TODO: Investigate making these public in Elmah.
    /// </summary>
    public static class ElmahHelper
    {
        public static T Find<T>(this IDictionary dict, object key, T @default)
        {
            if (dict == null) throw new ArgumentNullException("dict");
            return (T)(dict[key] ?? @default);
        }

        public static string GetConnectionString(IDictionary config)
        {
            //
            // First look for a connection string name that can be 
            // subsequently indexed into the <connectionStrings> section of 
            // the configuration to get the actual connection string.
            //

            string connectionStringName = config.Find("connectionStringName", string.Empty);

            if (connectionStringName.Length > 0)
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[connectionStringName];

                if (settings == null)
                    return string.Empty;

                return settings.ConnectionString ?? string.Empty;
            }

            //
            // Connection string name not found so see if a connection 
            // string was given directly.
            //

            var connectionString = config.Find("connectionString", string.Empty);
            if (connectionString.Length > 0)
                return connectionString;

            //
            // As a last resort, check for another setting called 
            // connectionStringAppKey. The specifies the key in 
            // <appSettings> that contains the actual connection string to 
            // be used.
            //

            var connectionStringAppKey = config.Find("connectionStringAppKey", string.Empty);
            return connectionStringAppKey.Length > 0
                 ? CloudConfigurationManager.GetSetting(connectionStringAppKey)
                 : string.Empty;
        }
    }
}
