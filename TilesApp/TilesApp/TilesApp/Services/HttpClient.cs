using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TilesApp.Models;

namespace TilesApp.Services
{
    class HttpClientManager
    {
        private static HttpClient _client = null;

        private HttpClientManager()
        {
        }

        public static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                }
                return _client;
            }
        }
        public static async Task<Location> ReverseGeoCodeAsync(string lat, string lon)
        {
            string uri = "https://nominatim.openstreetmap.org/reverse?format=json&lat="+lat+"&lon="+lon+"&zoom=18&addressdetails=1";
            try
            {
                var response =  await Client.GetAsync(uri);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<Location>(responseString);
                    return model;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // you need to maybe re-authenticate here
                    return default(Location);
                }
                else
                {
                    return default(Location);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
