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
                    HttpResponseMessage response = await client.GetAsync(BlackBoxesUri + App.CurrentProjectSlug + "/y/_families/__index");
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

        public async static Task<List<Web_Field>> GetFieldsList(string slug)
        {
            string result = "";
            List<Web_Field> fieldList = new List<Web_Field>();
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    HttpResponseMessage response = await client.GetAsync(BlackBoxesUri + App.CurrentProjectSlug + "/y/" + slug  + "/__show");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                        Dictionary<string, object> keyField = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

                        JArray ja = (JArray)keyField["protofamilyfields"];
                        List<Dictionary<string, object>> fieldDataList = ja.ToObject<List<Dictionary<string, object>>>();

                        foreach (Dictionary<string, object> fieldData in fieldDataList)
                        {
                            Web_Field field = new Web_Field();
                            if (fieldData["category"] != null) field.Category = Convert.ToInt32(fieldData["category"]);
                            if (fieldData["value_regex"] != null) field.ValueRegEx = fieldData["value_regex"].ToString();
                            if (fieldData["default"] != null) field.Default = fieldData["default"].ToString();
                            if (fieldData["primitive_quantity"] != null) field.PrimitiveQuantity = Convert.ToInt32(fieldData["primitive_quantity"]);
                            if (fieldData["entity_id"] != null) field.EntityId = fieldData["entity_id"].ToString();
                            if (fieldData["phases"] != null) field.Phases = fieldData["phases"].ToString();
                            if (fieldData["ui_index"] != null) field.UIindex = Convert.ToInt32(fieldData["ui_index"]);
                            if (fieldData["category"] != null) field.Category = Convert.ToInt32(fieldData["category"]);
                            if (fieldData["name"] != null) field.Name = fieldData["name"].ToString();
                            if (fieldData["long_name"] != null) field.LongName = fieldData["long_name"].ToString();
                            if (fieldData["description"] != null) field.Description = fieldData["description"].ToString();
                            if (fieldData["slug"] != null) field.Slug = fieldData["slug"].ToString();
                            if (fieldData["primitive_type"] != null) field.PrimitiveType = Convert.ToInt32(fieldData["primitive_type"]);
                            if (fieldData["value_is_unique"] != null) field.ValueIsUnique = Convert.ToInt32(fieldData["value_is_unique"]) == 1 ? true : false;
                            if (fieldData["value_is_required"] != null) field.ValueIsRequired = Convert.ToInt32(fieldData["value_is_required"]) == 1 ? true : false;
                            fieldList.Add(field);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", e.Message);
            }
            return fieldList;
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
    }
}
