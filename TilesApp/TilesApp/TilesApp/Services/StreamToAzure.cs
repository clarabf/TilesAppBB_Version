using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PCLAppConfig;
using Xamarin.Forms;

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
            Dictionary<string, string> resultDict = new Dictionary<string, string>();

            try
            {
                // Prepare blob connection. First client
                client = storageAccount.CreateCloudBlobClient();
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong. Could not connect to SACO Erp File Storage.");
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
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "App name is invalid. Could not create a container for the app to save files.");
            }

            try
            {
                outputBlob = container.GetBlockBlobReference(fileName);
                outputBlob.Properties.ContentType = "image/jpeg";
                outputBlob.UploadFromStreamAsync(fileStream).Wait();
                resultDict.Add("ContentMD5", outputBlob.Properties.ContentMD5);
                resultDict.Add("Uri", ConfigurationManager.AppSettings["AZURE_STORAGE_URL"] + "/" + appName.ToLower() + "/" + fileName);
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "File could not be saved. Content might be invalid or corrupt.");
            }
            return resultDict;
        }

        public static List<Dictionary<string, string>> WriteJPEGStreams(List<Stream> fileStreams, String appName)
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AZURE_STORAGE_CONNECTION_STRING"]);
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();

            try
            {
                // Prepare blob connection. First client
                client = storageAccount.CreateCloudBlobClient();
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong. Could not connect to SACO Erp File Storage.");
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
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "App name is invalid. Could not create a container for the app to save files.");
            }

            try
            {
                
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
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "File could not be saved. Content might be invalid or corrupt.");
            }
            return returnList;
        }

        public static Collection<string> UpdateJPEGStreams(List<Stream> fileStreams, String appName)
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AZURE_STORAGE_CONNECTION_STRING"]);
            Collection<string> returnList = new Collection<string>();

            try
            {
                // Prepare blob connection. First client
                client = storageAccount.CreateCloudBlobClient();
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong. Could not connect to SACO Erp File Storage.");
            }

            try
            {
                // Create container if not exits
                container = client.GetContainerReference(appName.ToLower().Replace(" ", ""));
                container.CreateIfNotExistsAsync().Wait();
                BlobContainerPermissions permissions = container.GetPermissionsAsync().Result;
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissionsAsync(permissions);
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "App name is invalid. Could not create a container for the app to save files.");
            }

            try
            {
                foreach (Stream str in fileStreams)
                {
                    string fileName = Guid.NewGuid().ToString() + ".jpeg";
                    outputBlob = container.GetBlockBlobReference(fileName);
                    outputBlob.Properties.ContentType = "image/jpeg";
                    outputBlob.UploadFromStreamAsync(str).Wait();
                    returnList.Add(ConfigurationManager.AppSettings["AZURE_STORAGE_URL"] + "/" + appName.ToLower() + "/" + fileName);
                }
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "File could not be saved. Content might be invalid or corrupt.");
            }
            return returnList;
        }
    }
}
