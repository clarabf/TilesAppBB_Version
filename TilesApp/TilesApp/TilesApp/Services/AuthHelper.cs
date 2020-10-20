using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TilesApp.Services
{
    public static class AuthHelper
    {
        #region CONFIGURATIONS
        private static string ClientID = "ec28d429-40c4-4ebc-8f8f-db9236df8830";
        private static string ClientSecret = "hfLdb5T_Chw]-yzjPLBd@9Fb3l7yaAA9";
        public static string UserScope = "User.Read";
        public static string OBOScope = "api://34f86a7a-fa1c-43d4-a97b-fe8dca310eef/test"; //TEST
        //public static string OBOScope = "https://blackboxes.azurewebsites.net/user_impersonation";
        #endregion

        #region PUBLIC METHODS
        public static async Task<bool> Login(string username, string password) 
        {
            try
            {
                //If there is a valid OBOToken stored, we login with it
                string oauthToken = await SecureStorage.GetAsync("oauth_token");
                if (oauthToken != null && oauthToken != "")
                {
                    if (FillDataWithOBOToken(oauthToken)) return true;
                }
                // We get User Access Token if there is none or has expired
                Dictionary<string, object> userToken = await LoginWithUsernameAndPassword(username, password);
                if (userToken.ContainsKey("access_token"))
                {
                    App.User.UserToken = (string)userToken["access_token"];
                    App.User.UserTokenExpiresAt = (DateTime)userToken["expires_at"];

                    // User info
                    string content = await GetHttpContentWithTokenAsync((string)userToken["access_token"]);
                    JObject user = JObject.Parse(content);

                    App.User.Email = username;
                    App.User.DisplayName = user["displayName"].ToString();
                    App.User.GivenName = user["givenName"].ToString();
                    App.User.MSID = user["id"].ToString();
                    App.User.Surname = user["surname"].ToString();
                    App.User.UserPrincipalName = user["userPrincipalName"].ToString();
                    
                    // OBO Token
                    Dictionary<string, object> oBOToken = await GetOBOToken(username, password);
                    if (oBOToken.ContainsKey("access_token"))
                    {
                        App.User.OBOToken = (string)oBOToken["access_token"];
                        App.User.OBOTokenExpiresAt = (DateTime)oBOToken["expires_at"];
                        App.User.LastLogIn = DateTime.Now;

                        //Store the OBOToken in Key Store
                        SecureStorage.Remove("oauth_token");
                        await SecureStorage.SetAsync("oauth_token", App.User.OBOToken);

                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool FillDataWithOBOToken(string oboToken)
        {
            bool success = false;
            
            try
            {
                String body = oboToken.Split('.')[1];
                
                //Fixing token length
                int mod4 = body.Length % 4;
                if (mod4 > 0)
                {
                    body += new string('=', 4 - mod4);
                }

                var encodedTextBytes = Convert.FromBase64String(body);
                string plainText = Encoding.UTF8.GetString(encodedTextBytes);
                Dictionary<string, object> tokenContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(plainText);

                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                App.User.OBOTokenExpiresAt = dtDateTime.AddSeconds(Convert.ToInt32(tokenContent["exp"])).ToLocalTime();
                App.User.OBOToken = oboToken;

                success = (App.IsConnected && CheckIfTokenIsValid()) || (!App.IsConnected) ? true : false;

                //Check it has not expired
                if (success)
                {
                    if (tokenContent.ContainsKey("unique_name"))
                    {
                        App.User.Email = tokenContent["unique_name"].ToString();
                        App.User.UserPrincipalName = tokenContent["unique_name"].ToString();
                    }
                    else if (tokenContent.ContainsKey("email"))
                    {
                        App.User.Email = tokenContent["email"].ToString();
                        App.User.UserPrincipalName = tokenContent["email"].ToString();
                    }
                    App.User.DisplayName = tokenContent["name"].ToString();
                    App.User.GivenName = tokenContent["given_name"].ToString();
                    App.User.MSID = tokenContent["oid"].ToString();
                    App.User.Surname = tokenContent["family_name"].ToString();
                    App.User.LastLogIn = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return success;
        }

        #endregion

        #region PRIVATE METHODS
        private static bool CheckIfTokenIsValid()
        {
            if (App.User.OBOToken == null)
            {
                return false;
            }
            else
            {
                try
                {
                    int res = DateTime.Compare(App.User.OBOTokenExpiresAt, DateTime.Now);
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
