using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PCLAppConfig;

namespace TilesApp.Services
{
    public static class StreamToAzure
    {
        private static CloudStorageAccount storageAccount;
        private static CloudBlobClient client;
        private static CloudBlobContainer container;
        private static CloudBlockBlob outputBlob;

        public static Dictionary<string, string> WriteJPEGStream(Stream fileStream, String appName)
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
                BlobContainerPermissions permissions = container.GetPermissionsAsync().Result;
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissionsAsync(permissions);
            }
            catch
            {
                throw new Exception("App name is invalid. Could not create a container for the app to save files.");
            }

            try
            {
                outputBlob = container.GetBlockBlobReference(fileName);
                outputBlob.Properties.ContentType = "image/jpeg";
                outputBlob.UploadFromStreamAsync(fileStream).Wait();

                return new Dictionary<string, string>()
                {
                    {"ContentMD5",outputBlob.Properties.ContentMD5},
                    {"Uri",ConfigurationManager.AppSettings["AZURE_STORAGE_URL"] + "/" + appName.ToLower() +"/" + fileName}
                };
            }
            catch
            {
                throw new Exception("File could not be saved. Content might be invalid or corrupt.");
            }
        }

        public static List<Dictionary<string, string>> WriteJPEGStreams(List<Stream> fileStreams, String appName)
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AZURE_STORAGE_CONNECTION_STRING"]);

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
                List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
                foreach (Stream str in fileStreams)
                {
                    string fileName = Guid.NewGuid().ToString() + ".jpeg";
                    outputBlob = container.GetBlockBlobReference(fileName);
                    outputBlob.Properties.ContentType = "image/jpeg";
                    outputBlob.UploadFromStreamAsync(str).Wait();
                    returnList.Add
                    (
                        new Dictionary<string, string>()
                        {
                            {"ContentMD5",outputBlob.Properties.ContentMD5},
                            {"Uri",ConfigurationManager.AppSettings["AZURE_STORAGE_URL"] + "/" + appName.ToLower() +"/" + fileName}
                        }
                    );
                }
                return returnList;
            }
            catch
            {
                throw new Exception("File could not be saved. Content might be invalid or corrupt.");
            }
        }
    }
}
