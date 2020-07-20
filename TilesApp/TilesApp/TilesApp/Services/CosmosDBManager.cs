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
using Newtonsoft.Json;
using PCLAppConfig;
using TilesApp.Models.DataModels;
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

        public static KeyValuePair<string, string> InsertOneObject(object data)
        {
            try
            {
                if (App.IsConnected)
                {
                    BsonDocument doc = data.ToBsonDocument();
                    collection.InsertOneAsync(doc).Wait();
                    return new KeyValuePair<string, string>("Success", "Online");
                }
                else
                {
                    PendingOperation opt = new PendingOperation();
                    opt.CreatedAt = DateTime.Now;
                    JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    };
                    opt.Data = JsonConvert.SerializeObject(data, microsoftDateFormatSettings);
                    opt.UserId = App.User.MSID;
                    opt.OperationType = data.GetType().Name;
                    opt.Station = App.Station;
                    opt.UserName = App.User.DisplayName;
                    opt.pha = 0;
                    App.Database.SavePendingOperation(opt);
                    return new KeyValuePair<string, string>("Success", "Offline");
                }
            }
            catch (Exception e)
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", e.Message);
                return new KeyValuePair<string, string>("Error", "1");
            }
        }

        public static void Init()
        {
            collection.CountDocumentsAsync(new BsonDocument());
        }

        public async static Task<List<Dictionary<string, object>>> FetchData(string barcode, Collection<string> Apps)
        {
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
