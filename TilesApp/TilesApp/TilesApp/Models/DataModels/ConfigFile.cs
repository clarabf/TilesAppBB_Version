using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Models.DataModels
{
    public class ConfigFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(50)]
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string AppType { get; set; }
    }
}
