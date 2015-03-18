using System.Data.Services.Common;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;
using System.IO.Compression;

namespace Elmah.AzureTableStorage
{
    public class ElmahEntity : TableEntity
    {
        public ElmahEntity()
        {
            // Azure Table Storage requires an empty constructor.
        }

        public ElmahEntity(string applicationName)
            : base(AzureHelper.EncodeAzureKey(applicationName), (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"))
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
        public byte[] AllXmlGZip { get; set; }

        public string GetXml()
        {
            //to stay Compatible with legacy uncompressed
            if (AllXmlGZip == null)
                return AllXml;

            using (var inStream = new MemoryStream(AllXmlGZip))
            using (var gzip = new GZipStream(inStream, CompressionMode.Decompress))
            using (var outStream = new MemoryStream())
            {
                gzip.CopyTo(outStream);
                return System.Text.Encoding.UTF8.GetString(outStream.ToArray());
            }
        }

        public void SetXml(string allXml)
        {

           var xmlBytes = System.Text.Encoding.UTF8.GetBytes(allXml);
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gzip.Write(xmlBytes, 0, xmlBytes.Length);
                    gzip.Close();
                }

                AllXmlGZip = ms.ToArray();
            }
        }

    }
}
