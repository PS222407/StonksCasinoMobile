using LibraryWindow.classes.Api;
using LibraryWindow.classes.Main;
using LibraryWindow.Classes.Lairsdice;
using LibraryWindow.Classes.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibraryWindow.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LiarDicePage : ContentPage
    {
        public string Username
        {
            get { return User.Username; }
        }

        public int Tokens
        {
            get { return User.Tokens; }
        }

        private Game _gameSetup = new Game(User.Username, 0);

        public Game GameSetup
        {
            get { return _gameSetup; }
            set { _gameSetup = value; OnPropertyChanged(); }
        }



        public bool LegalCall
        {
            get
            {
                if (int.Parse(SelectedItem) >= GameSetup.ActiveCall.DiceAmount && CallDice >= GameSetup.ActiveCall.DiceType)
                {
                    if (!(int.Parse(SelectedItem) == GameSetup.ActiveCall.DiceAmount && CallDice == GameSetup.ActiveCall.DiceType))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

        }

        private string _selectedItem = "1";

        public string SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(); OnPropertyChanged("LegalCall"); }
        }



        public string CallColor
        {
            get
            {
                if (LegalCall)
                {
                    return "White";
                }
                else
                {
                    return "Red";
                }
            }


        }



        private int _calldice;

        public int CallDice
        {
            get
            {
                return Math.Abs(_calldice % 6) + 1;
            }
            set { _calldice = value; OnPropertyChanged("DiceSource"); }
        }

        public ImageSource Cup { get; } = ImageSource.FromResource("LibraryWindow.Images.Lairsdice.cup2.png");

        public ImageSource DiceSource
        {
            get
            {
                return ImageSource.FromResource($"LibraryWindow.Images.Lairsdice.Dice.{CallDice}.png");
            }
        }



        public ImageSource background { get; } = ImageSource.FromResource("LibraryWindow.Images.Main.background.png");

        public LiarDicePage()
        {
            CallDice = 6000;
            InitializeComponent();
            BindingContext = this;
            OnPropertyChanged("DiceSource");
            Account();

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                OnPropertyChanged("Tokens");
                try
                {
                    ApiWrapper.GetUserInfo();
                }
                catch
                {
                    DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                    Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
                return true;
            });
        }



        private async void Account()
        {
            try
            {
                bool result = await ApiWrapper.GetUserInfo();
                OnPropertyChanged("Username");
                OnPropertyChanged("Tokens");
                if (!result)
                {
                    await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
            }
            catch
            {
                await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        private async void Uitloggen_Pressed(object sender, EventArgs e)
        {
            Preferences.Remove("user_name");
            Preferences.Remove("pass_word");
            Preferences.Remove("remember");

            try
            {
                bool logout = await User.LogoutAsync();
                if (logout)
                {
                    await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
            }
            catch
            {
                await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        [Obsolete]
        private void Store_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/winkel"));
        }

        [Obsolete]
        private void Deposit_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/account-info"));
        }

        private void Min_Pressed(object sender, EventArgs e)
        {
            _calldice--;
            OnPropertyChanged("DiceSource");
            OnPropertyChanged("LegalCall");
            OnPropertyChanged("CallColor");


        }
        private void Plus_Pressed(object sender, EventArgs e)
        {
            _calldice++;
            OnPropertyChanged("DiceSource");
            OnPropertyChanged("LegalCall");
            OnPropertyChanged("CallColor");

        }

        private async void Start_Pressed(object sender, EventArgs e)
        {
            try
            {
                string bet = await DisplayPromptAsync("Inzetten", "Hoeveel wilt u inzetten? u krijgt uw inleg x3 terug als u wint", keyboard: Keyboard.Numeric);
                if (int.Parse(bet) <= Tokens)
                {
                    if (int.Parse(bet) > 0)
                    {
                        await ApiWrapper.UpdateTokens(-int.Parse(bet), "Mobile LiarsDice, inzet");
                        Account();
                        GameSetup = new Game(Username, int.Parse(bet));
                        GameSetup.StartGame();
                        foreach (Player player in GameSetup.Players)
                        {
                            player.MyThrow.ThrowDice();
                        }
                        GameSetup.Players[0].CupOpacity = 0.3;
                        GameSetup.Startbtn = false;
                    }
                    else
                    {
                        await DisplayAlert("Te weinig inzet", "U heeft te weinig ingezet u moet meer dan 0 inzetten", "ok");
                    }
                }
                else
                {
                   await DisplayAlert("Te weinig tokens", "U heeft te weinig token stort tokens bij of zet lager in", "ok");
                }
            }
            catch 
            {

                
            }
           

           
            

        }

        private void Called_Pressed(object sender, EventArgs e)
        {
            if (LegalCall)
            {
                int DiceAmount = Int32.Parse(SelectedItem);
                GameSetup.SetActiveCall(DiceAmount, CallDice);
                GameSetup.NextTurn();
                OnPropertyChanged("LegalCall");
                OnPropertyChanged("CallColor");
                GameSetup.GameLogCall = $"{GameSetup.Players[0].Name} zegt:";
                GameSetup.GameLogSubtext = $"{DiceAmount} X {CallDice}";

            }
            else
            {
                DisplayAlert("Te lage inzet", $"Uw call moet hoger zijn dan {GameSetup.ActiveCall.DiceAmount} x {GameSetup.ActiveCall.DiceType}", "ok");
            }


        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("LegalCall");
            OnPropertyChanged("CallColor");
        }

        private void Liar_Pressed(object sender, EventArgs e)
        {
            GameSetup.Playerliar(GameSetup.CheckLiar());

        }
    }
}
