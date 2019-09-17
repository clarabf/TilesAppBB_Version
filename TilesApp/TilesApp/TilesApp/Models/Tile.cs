using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Models
{
    public class Tile
    {
        public int id { get; set; }
        public int work_order_id { get; set; }
        public int tile_type { get; set; }
        public string frame_code { get; set; }     

        public Tile(int work_order_id, int tile_type, string frame_code)
        {
            this.work_order_id = work_order_id;
            this.tile_type = tile_type;
            this.frame_code = String.IsNullOrEmpty(frame_code) ? "" : frame_code; 
        }

        public Tile()
        {

        }


    }
}
