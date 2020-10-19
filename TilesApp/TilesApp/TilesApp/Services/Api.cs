using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TilesApp.Models.DataModels;
using Xamarin.Forms;
using static System.Environment;

namespace TilesApp.Services
{
    public static class Api
    {

        //private static string BlackBoxesUri = "https://blackboxestest.azurewebsites.net/"; //TEST
        private static string BlackBoxesUri = "https://blackboxes.azurewebsites.net/";

        public async static Task<string> GetProjectsList()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    HttpResponseMessage response = await client.GetAsync(BlackBoxesUri + "_projects/__index");
                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();

                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }
        
        public async static Task<string> GetFamiliesList()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    //HttpResponseMessage response = await client.GetAsync(BlackBoxesUri + App.CurrentProjectSlug + "/y/_families/__index");
                    HttpResponseMessage response = await client.GetAsync(BlackBoxesUri + App.CurrentProjectSlug + "/r/_forms/__index");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public async static Task<string> GetFieldsList(string slug)
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    HttpResponseMessage response = await client.GetAsync(BlackBoxesUri + App.CurrentProjectSlug + "/r/" + slug  + "/__show");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", e.Message);
            }
            return result;
        }

        public async static Task<string> GetPhases()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    HttpResponseMessage response = await client.GetAsync(BlackBoxesUri + "_api/__getPhases");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public async static Task<string> GetPrimitiveTypes()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    HttpResponseMessage response = await client.GetAsync(BlackBoxesUri + "_api/__getPrimitiveTypes");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public async static Task<string> PostFormContent()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("test", "lala"),
                        new KeyValuePair<string, string>("test2", "lala2")
                    });
                    HttpResponseMessage response = await client.PostAsync(BlackBoxesUri + "3/test_form/__elements/__add", content);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public static List<Web_Field> FormatFields(string jsonFields)
        {

            List<Web_Field> fieldList = new List<Web_Field>();

            try
            {
                Dictionary<string, object> keyField = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFields);

                JArray ja = (JArray)keyField["protofamilyfields"];
                List<Dictionary<string, object>> fieldDataList = ja.ToObject<List<Dictionary<string, object>>>();

                foreach (Dictionary<string, object> fieldData in fieldDataList)
                {
                    Web_Field field = new Web_Field();
                    //if (fieldData.ContainsKey("category")) if (fieldData["category"]!= null) field.Category = Convert.ToInt32(fieldData["category"]);
                    if (fieldData["value_regex"] != null) field.ValueRegEx = fieldData["value_regex"].ToString();
                    if (fieldData["default"] != null) field.Default = fieldData["default"].ToString();
                    //if (fieldData["primitive_quantity"] != null) field.PrimitiveQuantity = Convert.ToInt32(fieldData["primitive_quantity"]);
                    if (fieldData["entity_id"] != null) field.EntityId = fieldData["entity_id"].ToString();
                    //if (fieldData["phases"] != null) field.Phases = fieldData["phases"].ToString();
                    if (fieldData["ui_index"] != null) field.UIindex = Convert.ToInt32(fieldData["ui_index"]);
                    if (fieldData["type"] != null) field.Type = Convert.ToInt32(fieldData["type"]);
                    if (fieldData["name"] != null) field.Name = fieldData["name"].ToString();
                    if (fieldData["long_name"] != null) field.LongName = fieldData["long_name"].ToString();
                    if (fieldData["description"] != null) field.Description = fieldData["description"].ToString();
                    if (fieldData["slug"] != null) field.Slug = fieldData["slug"].ToString();
                    if (fieldData["primitive_type"] != null)
                    {
                        field.PrimitiveType = Convert.ToInt32(fieldData["primitive_type"]);
                        PrimitiveType p = App.PrimitiveTypes[field.PrimitiveType.ToString()];
                        field.PrimitiveQuantity = p.Length;
                    }
                    if (fieldData["canWrite"] != null) field.CanWrite = (bool)fieldData["canWrite"];
                    if (fieldData["canRead"] != null) field.CanRead = (bool)fieldData["canRead"];
                    //if (fieldData["values_are_unique"] != null) field.ValueIsUnique = (bool)fieldData["value_is_unique"];
                    //if (fieldData["value_is_required"] != null) field.ValueIsRequired = Convert.ToInt32(fieldData["value_is_required"]) == 1 ? true : false;
                    fieldList.Add(field);
                }
            }
            catch {}
            return fieldList;
        }
    }
}
