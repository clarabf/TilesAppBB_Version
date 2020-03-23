using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

		async void OnLoginButtonClicked(object sender, EventArgs e)
		{
			//var user = new User
			//{
			//	Username = usernameEntry.Text,
			//	Password = passwordEntry.Text
			//};

			//var isValid = AreCredentialsCorrect(user);
			//if (isValid)
			//{
			//	App.IsUserLoggedIn = true;
			//	Navigation.InsertPageBefore(new MainPage(), this);
			//	await Navigation.PopAsync();
			//}
			//else
			//{
			//	messageLabel.Text = "Login failed";
			//	passwordEntry.Text = string.Empty;
			//}
		}

		bool AreCredentialsCorrect()
		{

			return true;
			//return user.Username == Constants.Username && user.Password == Constants.Password;
		}
	}
}