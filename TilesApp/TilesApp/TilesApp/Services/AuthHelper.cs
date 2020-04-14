using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TilesApp.Services
{
    public static class AuthHelper
    {
        #region CONFIGURATIONS
        private static string ClientID = "ec28d429-40c4-4ebc-8f8f-db9236df8830";
        private static string ClientSecret = "hfLdb5T_Chw]-yzjPLBd@9Fb3l7yaAA9";
        private static string TenantID = "970111b7-dfe6-4e9e-ab32-e335c6a60e65";

        // SACO
        //private static string ClientID = "109011d8-b3d4-46bb-98f8-5d017451a77f";
        //private static string ClientSecret = "CXKM7oyr[.uObJ1kRFTsMLEFQ-Ca.s40";
        //private static string TenantID = "a15915b3-ca25-4112-9ba7-0e850870a16b";

        public static string UserScope = "User.Read";
        public static string OBOScope = "api://7ee33ce2-39d6-446e-8522-846e5d39dd20/AccessApi";
        #endregion

        #region PUBLIC METHODS
        public static async Task<bool> Login(string username, string password) {
            // GET USER ACCESS TOKEN
            Dictionary<string, object> userToken = await LoginWithUsernameAndPassword(username, password);
            if (userToken.ContainsKey("access_token"))
            {
                App.User.UserToken = (string)userToken["access_token"];
                App.User.UserTokenExpiresAt = (DateTime)userToken["expires_at"];
                // GET USER INFO
                string content = await GetHttpContentWithTokenAsync((string)userToken["access_token"]);
                JObject user = JObject.Parse(content);
                try
                {
                    App.User.Email = username;
                    App.User.DisplayName = user["displayName"].ToString();
                    App.User.GivenName = user["givenName"].ToString();
                    App.User.MSID = user["id"].ToString();
                    App.User.Surname = user["surname"].ToString();
                    App.User.UserPrincipalName = user["userPrincipalName"].ToString();
                    // GET ON BEHALF OF USER ACCESS TOKEN
                    Dictionary<string, object> oBOToken = await GetOBOToken(username, password);
                    if (oBOToken.ContainsKey("access_token"))
                    {
                        App.User.Password = SHAEncription.GenerateSHA256String(password);
                        App.User.OBOToken = (string)oBOToken["access_token"];
                        App.User.OBOTokenExpiresAt = (DateTime)oBOToken["expires_at"];
                        App.User.LastLogIn = DateTime.Now;
                        App.Database.SaveUser(App.User);
                        return true;
                    }
                    else 
                    {
                        // there was an error
                        return false;
                    }
                   
                }
                catch (Exception)
                {
                    // there was an error
                    return false;
                }
            }
            else {
                // there was an error
                return false;
            }

        }

        public static bool CheckIfTokenIsValid(){
            if (App.User.UserToken == null)
            {
                return false;
            }
            else 
            {
                try
                {
                    int res = DateTime.Compare(App.User.UserTokenExpiresAt, DateTime.Now);
                    if (res >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

        }

        #endregion

        #region PRIVATE METHODS
        private static async Task<Dictionary<string, object>> LoginWithUsernameAndPassword(string username, string password)
        {
            JObject content = await RequestToken(username, password, UserScope);
            try
            {
                var token = content.ToObject<Dictionary<string, object>>();
                token.Add("expires_at", DateTime.Now.AddSeconds(Convert.ToInt32(token["expires_in"])));
                return token;
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }
        private static async Task<Dictionary<string,object>> GetOBOToken(string username, string password)
        {
            JObject content =  await RequestToken(username, password, OBOScope);
            var token = content.ToObject<Dictionary<string, object>>();
            try
            {
                token.Add("expires_at", DateTime.Now.AddSeconds(Convert.ToInt32(token["expires_in"])));
            }
            catch
            {
            }
            return token;
        }
        private static async Task<JObject> RequestToken(string username, string password, string scope)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://login.microsoftonline.com");
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("client_id", ClientID),
                        new KeyValuePair<string, string>("client_secret", ClientSecret),
                        new KeyValuePair<string, string>("scope", scope),
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password)
                    });
                    var result = await client.PostAsync("/organizations/oauth2/v2.0/token", content);
                    var response = await result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(response))
                    {
                        return JObject.Parse(response);
                    }
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }
        private static async Task<string> GetHttpContentWithTokenAsync(string token)
        {
            try
            {
                //get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK) return responseString;
                else return "error";
            }
            catch
            {
               return "error";
            }
        }
        #endregion
    }
}
