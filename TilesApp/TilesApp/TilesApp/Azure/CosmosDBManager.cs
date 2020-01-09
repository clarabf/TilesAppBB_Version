using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
using MongoDB.Driver;
using PCLAppConfig;

namespace TilesApp.Azure
{
    public static class CosmosDBManager
    {
        private static MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(ConfigurationManager.AppSettings["MONGODB_CONNECTION_STRING"])
            );
        private static MongoClient mongoClient = new MongoClient(settings);
        private static IMongoDatabase database = mongoClient.GetDatabase(ConfigurationManager.AppSettings["MONGODB_DB"]);
        private static IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(ConfigurationManager.AppSettings["MONGODB_COLLECTION"]);

        public static bool InsertOneObject(Dictionary<string, object> metaDataDictionary)
        {

            try
            {
                collection.InsertOneAsync(metaDataDictionary.ToBsonDocument()).Wait();
                return true;
            }
            catch
            {
                throw new Exception("Something went wrong. Could not connect save to database.");
            }
        }
    }
}
