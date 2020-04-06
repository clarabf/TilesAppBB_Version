using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp
{
    public class JSONParser
    {
        public string GenerateQCJSON()
        {

            Dictionary<string, object> application = new Dictionary<string, object>();
            Dictionary<string, object> appFields = new Dictionary<string, object>();
            Dictionary<string, object> predefinedFields = new Dictionary<string, object>();

            var fieldA = new Dictionary<string, object>()
            {
                {"FieldName", "ValidCodeFormat"},
                {"Description", "This field will be used to filter scanned items. Those matching filter will be added, others, ignored."},
                {"IsRequired", true},
                {"FieldIsSaved", false},
                {"FillableViaQR", false},
                {"Type", "String"},
                {"DefaultValue(admin)", new List<string>(){"00000002010700$$$$$$$$" }}
            };

            var fieldB = new Dictionary<string, object>()
            {
                {"FieldName", "Station"},
                {"Description", "This field is used to save the work station in which the app is being used by the employee. -IsRequired- field can be set to false if preferred."},
                {"IsRequired", true},
                {"FieldIsSaved", true},
                {"FillableViaQR", false},
                {"Type", "String"},
                {"DefaultValue(admin)", null}
            };

            var fieldC = new Dictionary<string, object>()
            {
                {"FieldName", "QCPass"},
                {"Description", "This field will be used to save the parent item UUID."},
                {"IsRequired", true},
                {"FieldIsSaved", true},
                {"FillableViaQR", true},
                {"Type", "Boolean"},
                {"DefaultValue(admin)", null}
            };

            var fieldD = new Dictionary<string, object>()
            {
                {"FieldName", "QCProcedureDetails"},
                {"Description", "This field is used to save QC procedure description and details. -IsRequired- field can be set to false if preferred."},
                {"IsRequired", true},
                {"FieldIsSaved", true},
                {"FillableViaQR", false},
                {"Type", "String"},
                {"DefaultValue(admin)", "Extrusion integrity visual test"}
            };

            var fieldE = new Dictionary<string, object>()
            {
                {"FieldName", "QCResultDetails"},
                {"Description", "This field is used to save additional data to the QC results. -IsRequired- field can be set to false if preferred."},
                {"IsRequired", true},
                {"FieldIsSaved", true},
                {"FillableViaQR", true},
                {"Type", "String"},
                {"DefaultValue(admin)", null }
            };

            var fieldF = new Dictionary<string, object>()
            {
                {"FieldName", "Images"},
                {"Description", "This field is used to save images to the QC results. -IsRequired- field can be set to false if preferred."},
                {"IsRequired", true},
                {"FieldIsSaved", true},
                {"FillableViaQR", false},
                {"Type", "String"},
                {"DefaultValue(admin)", new List<string>(){} }
            };

            appFields.Add("A", fieldA);
            appFields.Add("B", fieldB);
            appFields.Add("C", fieldC);
            appFields.Add("D", fieldD);
            appFields.Add("E", fieldE);
            appFields.Add("F", fieldF);
            application.Add("AppFields", appFields);

            application.Add("CustomFields", new Dictionary<string,object>());

            fieldA = new Dictionary<string, object>()
            {
                {"C", true },
                {"E", "No comments."},
            };

            fieldB = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "No comments."},
            };

            fieldC = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "Scratched."},
            };

            fieldD = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "Bad paint work."},
            };

            fieldE = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "Out of tolerance."},
            };

            fieldF = new Dictionary<string, object>()
            {
                {"C", false },
                {"E", "Mechanical damage."},
            };

            predefinedFields.Add("1A", fieldA);
            predefinedFields.Add("1B", fieldB);
            predefinedFields.Add("2A", fieldC);
            predefinedFields.Add("2B", fieldD);
            predefinedFields.Add("3A", fieldE);
            predefinedFields.Add("3B", fieldF);
            application.Add("PredefinedValues", predefinedFields);

            return JsonConvert.SerializeObject(application); ;
        }
    }
}
