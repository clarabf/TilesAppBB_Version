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
            string resource = "https://nominatim.openstreetmap.org/reverse?format=json&lat=";
            resource += lat;
            resource += "&lon=";
            resource += lon;
            resource += "&zoom=18&addressdetails=1";
            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(Client.BaseAddress, resource),
                    Method = HttpMethod.Get,
                };
                var response = await Client. SendAsync(request);
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

        public static Location ReverseGeoCode(string lat, string lon) {
            return ReverseGeoCodeAsync(lat, lon).Result;
        }
    }
}
