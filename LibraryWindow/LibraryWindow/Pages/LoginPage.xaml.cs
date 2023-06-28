using LibraryWindow.classes.Api;
using LibraryWindow.classes.Api.Models;
using LibraryWindow.classes.Main;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibraryWindow.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        bool blnShouldStay = true;
        protected override bool OnBackButtonPressed()
        {
            if (blnShouldStay)
            {
                // Yes, we want to stay.
                return true;
            }
            else
            {
                // It's okay, we can leave.
                base.OnBackButtonPressed();
                return false;
            }
        }


        private string _email;
        public string MyEmail
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password;
        public string MyPassword
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        private bool _remember = false;
        public bool Remember
        {
            get { return _remember; }
            set { _remember = value; OnPropertyChanged(); }
        }

        private bool fingerIsTrue = false;

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
            if (Preferences.Get("remember", "default_value") == "remembered")
            {
                fingerprint();
            }
            else
            {
                imgFinger.IsVisible = false;
                Grid.SetColumnSpan(btnLogin, 2);
            }
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            login();
        }

        private async void login()
        {
            try
            {
                LoginCredentials credentials = new LoginCredentials() { Email = MyEmail, Password = MyPassword, Overwride = false };
                string result = await ApiWrapper.Login(credentials);
                User.Logoutclick = false;

                if (result == "succes")
                {
                    if (Remember)
                    {
                        RememberMe();
                    }
                    //await Navigation.PopAsync();
                    //await Navigation.PopToRootAsync();
                    await Navigation.PushModalAsync(new NavigationPage(new MainPage()));
                }
                else if (result == "active")
                {
                    bool action = false;
                    if (!fingerIsTrue)
                    {
                        action = await App.Current.MainPage.DisplayAlert($"Opgelet!", "Er is al iemand anders ingelogd op dit account! Als u toch wilt inloggen wordt de ander van uw account afgezet. Let op! Dit kan nadelige gevolgen hebben voor uw account als de persoon die ingelogd is momenteel bezig is met een spel heb je het risico om je inzit kwijt te raken. Wilt u toch inloggen?", "Yes", "No");
                    }
                    if (action || fingerIsTrue)
                    {
                        credentials = new LoginCredentials() { Email = MyEmail, Password = MyPassword, Overwride = true };
                        result = await ApiWrapper.Login(credentials);
                        if (Remember)
                        {
                            RememberMe();
                        }
                        //await Navigation.PopAsync();
                        //await Navigation.PopToRootAsync();
                        await Navigation.PushModalAsync(new NavigationPage(new MainPage()));
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Gebruikersnaam of wachtwoord is incorrect.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
            }
        }

        private void RememberMe()
        {
            Preferences.Set("user_name", MyEmail);
            Preferences.Set("pass_word", MyPassword);
            Preferences.Set("remember", "remembered");

        }

        [Obsolete]
        private void Passreset_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/wachtwoord"));
        }

        [Obsolete]
        private void Register_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/register"));
        }


        private void LoginRemember()
        {
            //try
            //{
            //    var checkPreferences = Preferences.Get("remember", "default_value");
            //    var checkPreferences1 = Preferences.Get("user_name", "default_value");
            //    var checkPreferences2 = Preferences.Get("pass_word", "default_value");

            //    if (checkPreferences == "true")
            //    {
            //        //LoginCredentials credentials = new LoginCredentials() { Email = Preferences.Get("user_name", "default_value"), Password = Preferences.Get("pass_word", "default_value"), Overwride = false };
            //        //string result = await ApiWrapper.Login(credentials);

            //        //if (result == "active")
            //        //{
            //        Navigation.PushModalAsync(new NavigationPage(new MainPage()));
            //        //}
            //    }
            //}
            //catch
            //{

            //}
        }

        private async void fingerprint()
        {
            var Isavaliable = await CrossFingerprint.Current.IsAvailableAsync();
            if (!Isavaliable)
            {
                await DisplayAlert("Helaas", "Er is geen vingerafdruk geregistreerd op dit apparaat. Ga naar de instellingen van je apparaat om een vingerafdruk toe te voegen.", "Ok");
                return;
            }
            var AuthFinger = await CrossFingerprint.Current.AuthenticateAsync
                (new AuthenticationRequestConfiguration("Biometrie vergrendeling", "Gebruik vingerafdruk of gezichtsherkenning om in te loggen."));
            if (AuthFinger.Authenticated)
            {
                fingerIsTrue = true;

                MyEmail = Preferences.Get("user_name", "default_value");
                MyPassword = Preferences.Get("pass_word", "default_value");

                login();
            }
        }

        private void FingerprintButton_Pressed(object sender, EventArgs e)
        {
            fingerprint();
        }
    }
}