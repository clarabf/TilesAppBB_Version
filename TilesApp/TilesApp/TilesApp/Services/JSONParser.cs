using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TilesApp.Models;
using TilesApp.Models.DataModels;
using TilesApp.Models.Skeletons;

namespace TilesApp
{
    public class JSONParser
    {
        public string Join_JSON()
        {

            Dictionary<string, object> application = new Dictionary<string, object>();
            Dictionary<string, object> appFields = new Dictionary<string, object>();
            Dictionary<string, object> customFields = new Dictionary<string, object>();
            Dictionary<string, object> predefinedFields = new Dictionary<string, object>();

            //@ALAA check the barcode code is correct
            appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() {"8051739305029", "045496470289"}));
            //appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() {"00000002010700$$$$$$$$"})) ;
            appFields.Add("B", AppField("Station", "Station Description", true, true, true, "String", null));
            appFields.Add("C", AppField("ParentUUID", "ParentUUID Description", true, true, true, "String", null));
            appFields.Add("D", AppField("ParentCodeFormat", "ParentCodeFormat Description", true, false, false, "String", "045496470289"));
            application.Add("AppFields", appFields);

            application.Add("CustomFields", customFields);
            application.Add("PredefinedValues", predefinedFields);

            return JsonConvert.SerializeObject(application); ;
        }
        public string Review_JSON()
        {

            Dictionary<string, object> application = new Dictionary<string, object>();
            Dictionary<string, object> appFields = new Dictionary<string, object>();
            Dictionary<string, object> customFields = new Dictionary<string, object>();
            Dictionary<string, object> predefinedFields = new Dictionary<string, object>();

            //@ALAA check the barcode code is correct
            appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() { "8051739305029", "045496470289" }));
            //appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() {"00000002010700$$$$$$$$"})) ;
            appFields.Add("B", AppField("Station", "Station Description", true, true, true, "String", null));
            appFields.Add("C", AppField("Apps", "Apps Description", true, true, false, "String", new List<string>() { "testQC", "testLink", "testJoin" }));
            application.Add("AppFields", appFields);

            application.Add("CustomFields", customFields);
            application.Add("PredefinedValues", predefinedFields);

            return JsonConvert.SerializeObject(application); ;
        }
        public string Link_JSON()
        {
            Dictionary<string, object> application = new Dictionary<string, object>();
            Dictionary<string, object> appFields = new Dictionary<string, object>();
            Dictionary<string, object> customFields = new Dictionary<string, object>();
            Dictionary<string, object> predefinedFields = new Dictionary<string, object>();

            //@ALAA check the barcode code is correct
            appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() { "8051739305029" }));
            //appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() {"00000002010700$$$$$$$$"})) ;
            appFields.Add("B", AppField("Station", "Station description.", true, true, true, "String", "Prod Workstation Alfa"));
            application.Add("AppFields", appFields);

            customFields.Add("1", CustomField("ProductFamily", "ProductFamily Description", false, false, null));
            customFields.Add("2", CustomField("ProductType", "ProductType Description", false, false, null));
            customFields.Add("3", CustomField("ProductModel", "ProductModel Description", false, false, null));
            customFields.Add("4", CustomField("ProductGeneration", "ProductGeneration Description", false, false, null));
            customFields.Add("5", CustomField("ProductPartNr", "ProductPartNr Description", false, false, null));
            customFields.Add("6", CustomField("ProductInfo", "ProductInfo Description", false, false, null));
            customFields.Add("7", CustomField("ProductCatalogueURL", "ProductCatalogueURL Description", false, false, null));
            customFields.Add("8", CustomField("ProductManualURL", "ProductManualURL Description", false, false, null));
            application.Add("CustomFields", customFields);

            predefinedFields.Add("1A", SACOProductsInfo("Vegas Version", "1.0"));
            predefinedFields.Add("1B", SACOProductsInfo("London Version", "1.0"));
            predefinedFields.Add("2A", SACOProductsInfo("Vegas Version", "1.1"));
            predefinedFields.Add("2B", SACOProductsInfo("London Version", "1.1"));
            predefinedFields.Add("3A", SACOProductsInfo("Vegas Version", "1.2"));
            predefinedFields.Add("3B", SACOProductsInfo("London Version", "1.2"));
            predefinedFields.Add("4A", SACOProductsInfo("Vegas Version", "1.3"));
            predefinedFields.Add("4B", SACOProductsInfo("London Version", "1.3"));
            predefinedFields.Add("5A", SACOProductsInfo("Vegas Version", "1.4"));
            predefinedFields.Add("5B", SACOProductsInfo("London Version", "1.4"));
            predefinedFields.Add("6A", SACOProductsInfo("Vegas Version", "2.0"));
            predefinedFields.Add("6B", SACOProductsInfo("London Version", "2.0"));
            predefinedFields.Add("7A", SACOProductsInfo("Vegas Version", "2.1"));
            predefinedFields.Add("7B", SACOProductsInfo("London Version", "2.1"));
            predefinedFields.Add("8A", SACOProductsInfo("Vegas Version", "2.2"));
            predefinedFields.Add("8B", SACOProductsInfo("London Version", "2.2"));
            predefinedFields.Add("9A", SACOProductsInfo("Vegas Version", "2.3"));
            predefinedFields.Add("9B", SACOProductsInfo("London Version", "2.3"));
            application.Add("PredefinedValues", predefinedFields);

            return JsonConvert.SerializeObject(application); ;
        }
        public string QC_JSON()
        {

            Dictionary<string, object> application = new Dictionary<string, object>();
            Dictionary<string, object> appFields = new Dictionary<string, object>();
            Dictionary<string, object> customFields = new Dictionary<string, object>();
            Dictionary<string, object> predefinedFields = new Dictionary<string, object>();

            //@ALAA check the barcode code is correct
            appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() { "8051739305029" }));
            //appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() {"00000002010700$$$$$$$$"})) ;
            appFields.Add("B", AppField("Station", "Station Description", true, true, false, "String", null));
            appFields.Add("C", AppField("QCPass", "QCPass Description", true, true, true, "Boolean", null));
            appFields.Add("D", AppField("QCProcedureDetails", "QCProcedureDetails Description", true, true, false, "String", "Extrusion integrity visual test."));
            appFields.Add("E", AppField("QCResultDetails", "QCResultDetails Description", true, true, true, "String", "Bla bla bla"));
            appFields.Add("F", AppField("Images", "Images Description", true, true, false, "String", new List<string>() {}));
            application.Add("AppFields", appFields);

            application.Add("CustomFields", customFields);

            var field1A = new Dictionary<string, object>()
            {
                {"C", true },
                {"E", "No comments."}
            };

            var field1B = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "No comments."}
            };

            var field2A = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "Scratched."}
            };

            var field2B = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "Bad paint work."}
            };

            var field3A = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "Out of tolerance."},
            };

            var field3B = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "Mechanical damage."},
            };

            predefinedFields.Add("1A", field1A);
            predefinedFields.Add("1B", field1B);
            predefinedFields.Add("2A", field2A);
            predefinedFields.Add("2B", field2B);
            predefinedFields.Add("3A", field3A);
            predefinedFields.Add("3B", field3B);
            application.Add("PredefinedValues", predefinedFields);

            return JsonConvert.SerializeObject(application); ;
        }
        public string Reg_JSON()
        {

            Dictionary<string, object> application = new Dictionary<string, object>();
            Dictionary<string, object> appFields = new Dictionary<string, object>();
            Dictionary<string, object> customFields = new Dictionary<string, object>();
            Dictionary<string, object> predefinedFields = new Dictionary<string, object>();

            //@ALAA check the barcode code is correct
            appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() { "8051739305029" }));
            //appFields.Add("A", AppField("ValidCodeFormat", "ValidCodeFormar description.", true, false, false, "String", new List<string>() {"00000002010700$$$$$$$$"})) ;
            appFields.Add("B", AppField("Station", "Station Description", true, true, true, "String", null));
            appFields.Add("C", AppField("Registry", "Registry Description", true, true, true, "String", null));
            appFields.Add("D", AppField("RegistryDetails", "RegistryDetails Description", true, true, true, "String", null));
            application.Add("AppFields", appFields);

            application.Add("CustomFields", customFields);

            var field1A = new Dictionary<string, object>()
            {
                {"C", "Completed" },
                {"D", "No comments"}
            };

            var field1B = new Dictionary<string, object>()
            {
                {"C", "To be completed" },
                {"D", "No comments"}
            };

            var field1C = new Dictionary<string, object>()
            {
                {"C", "Not completed" },
                {"D", "Missing material"}
            };

            var field1D = new Dictionary<string, object>()
            {
                {"C", "Not completed" },
                {"D", "Faulty material"}
            };

            var field1E = new Dictionary<string, object>()
            {
                {"C", "Not completed" },
                {"D", "QC Failed"}
            };

            predefinedFields.Add("1A", field1A);
            predefinedFields.Add("1B", field1B);
            predefinedFields.Add("1C", field1C);
            predefinedFields.Add("1D", field1D);
            predefinedFields.Add("1E", field1E);
            application.Add("PredefinedValues", predefinedFields);

            return JsonConvert.SerializeObject(application); ;
        }

        private Dictionary<string, object> AppField(string fName, string fDescription, bool fIsRequired, 
                                                    bool fIsSaved, bool fIsFillable, string fType, object fDefaultValue)
        {
            return new Dictionary<string, object>()
            {
                {"FieldName", fName},
                {"Description", fDescription},
                {"IsRequired", fIsRequired},
                {"FieldIsSaved", fIsSaved},
                {"FillableViaQR", fIsFillable},
                {"Type", fType},
                {"DefaultValue(admin)", fDefaultValue}
            };
        }
        private Dictionary<string, object> CustomField(string fName, string fDescription, bool fIsRequired, 
                                                       bool fIsFillable, object fDefaultValue)
        {
            return new Dictionary<string, object>()
            {
                {"FieldName(admin)", fName},
                {"Description(admin)", fDescription},
                {"IsRequired(admin)", fIsRequired},
                {"FillableViaQR(admin)", fIsFillable},
                {"DefaultValue(admin)", fDefaultValue}
            };
        }
        private Dictionary<string, object> SACOProductsInfo(string place, string version)
        {
            return new Dictionary<string, object>()
            {
                {"1", "Shockwave"},
                {"2", "Puck"},
                {"3", place},
                {"4", version},
                {"5", "SAC9e1e7"},
                {"6", "RGB 110 Deg"},
                {"7", "http:\\www.google.es"},
                {"8", "http:\\www.google.es"}
            };
        }

        public static object JsonToOperation(PendingOperation opt)
        {
            object obj = new object();
            switch (opt.OperationType)
            {
                case "JoinMetaData":
                    obj = JsonConvert.DeserializeObject<JoinMetaData>(opt.Data);
                    break;
                case "LinkMetaData":
                    obj = JsonConvert.DeserializeObject<LinkMetaData>(opt.Data);
                    break;
                case "QCMetaData":
                    obj = JsonConvert.DeserializeObject<QCMetaData>(opt.Data);
                    break;
                case "RegMetaData":
                    obj = JsonConvert.DeserializeObject<RegMetaData>(opt.Data);
                    break;
                case "ReviewMetaData":
                    obj = JsonConvert.DeserializeObject<ReviewMetaData>(opt.Data);
                    break;
                case "AppBasicOperation":
                    obj = JsonConvert.DeserializeObject<AppBasicOperation>(opt.Data);
                    break;
                default:
                    obj = opt.Data;
                    break;
            }
            return obj;
        }
    }
}
