using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Models
{
    public class TileTask
    {

        public int id { get; set; }
        public int tile_id { get; set; }
        public int step_id { get; set; }
        public string assigned_worker { get; set; }
        public int current_status { get; set; }

        public TileTask(int tile_id, int step_id, string assigned_worker )
        {
            this.tile_id = tile_id;
            this.step_id = step_id;
            this.assigned_worker = String.IsNullOrEmpty(assigned_worker) ? "" : assigned_worker; ;
            this.current_status = 1;
        }
        public TileTask()
        {
            this.current_status = 1;
        }
    }
}
