using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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

        public static void Init() {
            collection.CountDocumentsAsync(new BsonDocument());
        }

        public async static Task<List<Dictionary<string, object>>> FetchData(string barcode, Collection<string> Apps) {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                // Fetch data from cosmo
                var filter = $"{{\"AppName\": {{$in:['{string.Join("','", Apps)}'] }}, ScannerReads: {{ $elemMatch: {{ Value : \"{barcode}\" }}}}}}";
                await collection.Find(filter).ForEachAsync(document => {
                    dict = BsonSerializer.Deserialize<Dictionary<string, object>>(document);
                    result.Add(dict);
                });              
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                
            }

            return result;
        }

    }
}
