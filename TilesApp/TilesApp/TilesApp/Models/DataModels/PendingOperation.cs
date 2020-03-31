using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Models.DataModels
{
    public class PendingOperation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(50)]
        public DateTime CreatedAt { get; set; }
        public DateTime SavedAt { get; set; }
        public int UserId { get; set; }
        public string Data { get; set; }
    }
}
