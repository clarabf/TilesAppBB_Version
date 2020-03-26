using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TilesApp.Models
{
    public class UserData
    {
        #region PRIVATE VARIABLES
        private string _displayName;
        private string _givenName { get; set; }
        private string _surname { get; set; }
        private string _userPrincipalName { get; set; }
        private string _email { get; set; }
        private string _id { get; set; }
        private Dictionary<string, object> _userToken { get; set; }
        private Dictionary<string, object> _oBOToken { get; set; }
        #endregion
        public string DisplayName { 
            get 
            {
                if (Application.Current.Properties.ContainsKey("DisplayName"))
                {
                    string displayName = Application.Current.Properties["DisplayName"] as string;
                    if (displayName != null && displayName != "") _displayName = displayName;
                }
                return _displayName;
            }
            set 
            {
                _displayName = value;
                Application.Current.Properties["DisplayName"] = value;
            } 
        }
        public string GivenName
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("GivenName"))
                {
                    string givenName = Application.Current.Properties["GivenName"] as string;
                    if (givenName != null && givenName != "") _givenName = givenName;
                }
                return _givenName;
            }
            set
            {
                _givenName = value;
                Application.Current.Properties["GivenName"] = value;
            }
        }
        public string Surname
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("Surname"))
                {
                    string surname = Application.Current.Properties["Surname"] as string;
                    if (surname != null && surname != "") _surname = surname;
                }
                return _surname;
            }
            set
            {
                _surname = value;
                Application.Current.Properties["Surname"] = value;
            }
        }
        public string UserPrincipalName
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("UserPrincipalName"))
                {
                    string userPrincipalName = Application.Current.Properties["UserPrincipalName"] as string;
                    if (userPrincipalName != null && userPrincipalName != "") _userPrincipalName = userPrincipalName;
                }
                return _userPrincipalName;
            }
            set
            {
                _userPrincipalName = value;
                Application.Current.Properties["UserPrincipalName"] = value;
            }
        }
        public string Email
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("Email"))
                {
                    string email = Application.Current.Properties["Email"] as string;
                    if (email != null && email != "") _email = email;
                }
                return _email;
            }
            set
            {
                _email = value;
                Application.Current.Properties["Email"] = value;
            }
        }
        public string ID
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("ID"))
                {
                    string id = Application.Current.Properties["ID"] as string;
                    if (id != null && id != "") _id = id;
                }
                return _id;
            }
            set
            {
                _id = value;
                Application.Current.Properties["ID"] = value;
            }
        }
        public Dictionary<string, object> UserToken
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("UserToken"))
                {
                    Dictionary<string, object> userToken = Application.Current.Properties["UserToken"] as Dictionary<string,object>;
                    if (userToken != null && userToken.Count >0) _userToken = userToken;
                }
                return _userToken;
            }
            set
            {
                _userToken = value;
                Application.Current.Properties["UserToken"] = value;
            }
        }
        public Dictionary<string, object> OBOToken
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("OBOToken"))
                {
                    Dictionary<string, object> oBOToken = Application.Current.Properties["UserToken"] as Dictionary<string, object>;
                    if (oBOToken != null && oBOToken.Count > 0) _oBOToken = oBOToken;
                }
                return _oBOToken;
            }
            set
            {
                _oBOToken = value;
                Application.Current.Properties["OBOToken"] = value;
            }
        }

    }
}
