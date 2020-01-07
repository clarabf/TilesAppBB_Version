using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PCLAppConfig;

namespace TilesApp.Azure
{
    public static class StreamToAzure
    {
        private static CloudStorageAccount storageAccount;
        private static CloudBlobClient client;
        private static CloudBlobContainer container;
        private static CloudBlockBlob outputBlob;

        public static Dictionary<string,string> WriteJPEGStream(Stream fileStream, String appName, Dictionary<string,string> metaDataDictionary)
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AZURE_STORAGE_CONNECTION_STRING"]);
            string fileName = Guid.NewGuid().ToString() + ".jpeg";

            try
            {
                // Prepare blob connection. First client
                client = storageAccount.CreateCloudBlobClient();
            }
            catch
            {
                throw new Exception("Something went wrong. Could not connect to SACO Erp File Storage.");
            }

            try
            {
                // Create container if not exits
                container = client.GetContainerReference(appName.ToLower());
                container.CreateIfNotExistsAsync().Wait();
            }
            catch
            {
                throw new Exception("App name is invalid. Could not create a container for the app to save files.");
            }

            try
            {
                outputBlob = container.GetBlockBlobReference(fileName);
                outputBlob.Properties.ContentType = "image/jpeg";
                foreach (var key in metaDataDictionary.Keys)
                {
                    outputBlob.Metadata.Add(key, metaDataDictionary[key]);
                }
                outputBlob.UploadFromStreamAsync(fileStream).Wait();
            }
            catch
            {
                throw new Exception("File could not be saved. Content might be invalid or corrupt.");
            }
            
            return new Dictionary<string, string>()
            {
                {"FileName", fileName},
                {"ContentMD5",outputBlob.Properties.ContentMD5}
            };
        }
    }
}
