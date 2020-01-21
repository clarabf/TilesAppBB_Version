using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Models
{
    public class Location
    {
        public int place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public int osm_id { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
        public Dictionary<string,string> address { get; set; }
        public string[] boundingbox { get; set; }
    }
}
