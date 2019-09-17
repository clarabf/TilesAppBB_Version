using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Models
{
    public class Step
    {
        public int id { get; set; }
        public int tile_type { get; set; }
        public int step_order { get; set; }
        public string url { get; set; }

        public Step(int tile_type, int step_order, string url)
        {
            this.tile_type = tile_type;
            this.step_order = step_order;
            this.url = String.IsNullOrEmpty(url) ? "" : url; ;
        }

        public Step()
        {

        }
    }
}
