using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp
{
    public class JSONParser
    {
        public string GenerateJSON()
        {
            string json = "";

            Dictionary<string, object> application = new Dictionary<string, object>();
            List<object> elements = new List<object>();
            Dictionary<string, object> element;
            List<object> comboBoxElements = new List<object>();
            Dictionary<string, object> comboBoxItem;

            //Add Barcode
            element = new Dictionary<string, object>();
            element.Add("type", "Barcode");
            element.Add("topText", "Text above the scanner");
            element.Add("bottomText", "Text under the scanner");
            elements.Add(element);

            //Add ComboBox
            comboBoxItem = new Dictionary<string, object>();
            comboBoxItem.Add("name", "item1");
            comboBoxElements.Add(comboBoxItem);
            comboBoxItem = new Dictionary<string, object>();
            comboBoxItem.Add("name", "item2");
            comboBoxElements.Add(comboBoxItem);
            comboBoxItem = new Dictionary<string, object>();
            comboBoxItem.Add("name", "item3");
            comboBoxElements.Add(comboBoxItem);

            element = new Dictionary<string, object>();
            element.Add("type", "ComboBox");
            element.Add("title", "Title of the ComboBox");
            element.Add("elements", comboBoxElements);
            elements.Add(element);

            //Add Text
            element = new Dictionary<string, object>();
            element.Add("type", "Text");
            element.Add("color", "#f8f8ff");
            element.Add("fontsize", 22);
            element.Add("content", "Text showed");
            elements.Add(element);

            application.Add("title", "TITULO APP");
            application.Add("elements", elements);

            json = JsonConvert.SerializeObject(application);

            return json;
        }
    }
}
