using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TilesApp.Models.DataModels
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(50)]
        public string DisplayName { get; set; }
        [MaxLength(50)]
        public string GivenName {get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(50)]
        public string UserPrincipalName { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(30)]
        public string MSID { get; set; }
        public string Password { get; set; }
        public string UserToken { get; set; }
        public DateTime UserTokenExpiresAt { get; set; }
        public string OBOToken { get; set; }
        public DateTime OBOTokenExpiresAt { get; set; }
        public DateTime LastLogIn { get; set; }

    }
}
