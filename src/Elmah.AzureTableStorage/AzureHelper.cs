using System;
using System.Text;

namespace Elmah.AzureTableStorage
{
    /// <summary>
    ///     Includes methods to encode or decode values for Azure key fields.
    /// </summary>
    /// <remarks>
    ///     Original source - http://msdn.microsoft.com/en-us/library/ff803362.aspx
    /// 
    ///     Azure places some restrictions on the characters that you can use in partition and row keys.
    /// 
    ///     The following information is copied from: http://msdn.microsoft.com/en-us/library/dd179338.aspx
    /// 
    ///     The following characters are not allowed in values for the PartitionKey and RowKey properties:
    /// 
    ///     The forward slash (/) character
    ///     The backslash (\) character
    ///     The number sign (#) character
    ///     The question mark (?) character
    ///     Control characters from U+0000 to U+001F, including:
    ///     The horizontal tab (\t) character
    ///     The linefeed (\n) character
    ///     The carriage return (\r) character
    ///     Control characters from U+007F to U+009F
    /// </remarks>
    internal class AzureHelper
    {
        internal static string EncodeAzureKey(string key)
        {
            return key == null ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
        }

        internal static string DecodeAzureKey(string encodedKey)
        {
            return encodedKey == null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(encodedKey));
        }
    }
}