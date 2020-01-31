using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
using MongoDB.Driver;
using PCLAppConfig;
using Xamarin.Forms;

namespace TilesApp.Services
{
    public static class CosmosDBManager
    {
        private static MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(ConfigurationManager.AppSettings["MONGODB_CONNECTION_STRING"])
            );
        private static MongoClient mongoClient = new MongoClient(settings);
        private static IMongoDatabase database = mongoClient.GetDatabase(ConfigurationManager.AppSettings["MONGODB_DB"]);
        private static IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(ConfigurationManager.AppSettings["MONGODB_COLLECTION"]);

        public static bool InsertOneObject(object metaData)
        {            
            try
            {
                BsonDocument doc = metaData.ToBsonDocument();
                collection.InsertOneAsync(doc).Wait();
                return true;
            }
            catch(Exception e)
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", e.Message);
                return false;
            }
        }

        public static void init() {
            collection.CountDocumentsAsync(new BsonDocument());
        }
    }
}
