﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
                container = client.GetContainerReference("qcimgs");
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
                    string fileName = appName.ToLower().Replace(" ", "") + "/" + Guid.NewGuid().ToString() + ".jpeg";
                    outputBlob = container.GetBlockBlobReference(fileName);
                    outputBlob.Properties.ContentType = "image/jpeg";
                    outputBlob.UploadFromStreamAsync(str).Wait();
                    returnList.Add(ConfigurationManager.AppSettings["AZURE_STORAGE_URL"] + "qcimgs/" + fileName);
                }
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "File could not be saved. Content might be invalid or corrupt.");
            }
            return returnList;
        }

        public static async Task<string> GetLastestAPKAsync()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AZURE_STORAGE_CONNECTION_STRING"]);
            string filePath = "";
            try
            {
                client = storageAccount.CreateCloudBlobClient();
                container = client.GetContainerReference("containertest");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return filePath;
            }
            string apkName = "Sherpa.apk";
            var blob = container.GetBlockBlobReference("apk/latest/"+ apkName);
            if (await blob.ExistsAsync())
            {
                string completeName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), apkName);

                using (var fileStream = File.OpenWrite(completeName))
                {
                    Console.WriteLine("Test");
                    try
                    {
                        await blob.DownloadToStreamAsync(fileStream);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.ToString()}");
                        return filePath;
                    }
                }

                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), apkName)) == true)
                {
                    Console.WriteLine("You got it!");
                    filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), apkName);
                    return filePath;
                }
                else
                {
                    Console.WriteLine("No File!!!");
                    return filePath;
                }
            }
            return filePath;
        }

    }
}
