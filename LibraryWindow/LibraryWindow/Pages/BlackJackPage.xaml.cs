using LibraryWindow.classes.Api;
using LibraryWindow.classes.Main;
using LibraryWindow.Classes.BlackJack;
using LibraryWindow.Classes.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class BlackJackPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Username
        {
            get { return User.Username; }
        }

        public int Tokens
        {
            get { return User.Tokens; }
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




        private const string _sender = "Blackjack";

        private CardBlackjack _cardturned;

        public CardBlackjack Mycardturned
        {
            get { return _cardturned; }
            set { _cardturned = value; OnPropertyChanged(); }
        }

        private BlackjackDeck deck = new BlackjackDeck();

        private Computers _player;

        public Computers Playergame
        {
            get { return _player; }
            set { _player = value; OnPropertyChanged(); }
        }

        private Computers _computer;

        public Computers ComputerGame
        {
            get { return _computer; }
            set { _computer = value; OnPropertyChanged(); }
        }

        private Computers _computer2;

        public Computers ComputerGame2
        {
            get { return _computer2; }
            set { _computer2 = value; OnPropertyChanged(); }
        }

        private bool _splitfunction = false;

        public bool Splitfunction
        {
            get { return _splitfunction; }
            set { _splitfunction = value; OnPropertyChanged(); }
        }

        private bool _deckswitch = false;

        public bool Deckswitch
        {
            get { return _deckswitch; }
            set { _deckswitch = value; OnPropertyChanged(); }
        }

        private bool _dubbelentrue;

        public bool Dubbelentrue
        {
            get { return _dubbelentrue; }
            set { _dubbelentrue = value; }
        }


        private BlackJacken _game = new BlackJacken();

        public BlackJacken Game
        {
            get { return _game; }
            set { _game = value; OnPropertyChanged(); }
        }

        public BlackJackPage()
        {
            BlackjackWindowRestart();
            InitializeComponent();
            BindingContext = this;
            Account();

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
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

        private void BlackjackWindowRestart()
        {
            Game = new BlackJacken();
            ComputerGame = new Computers();
            Game.Blackjackwindow();
            Game.MyAantal = Aantalonthouden;
        }

        private Computers _computers = new Computers();

        public Computers MyComputers
        {
            get { return _computers; }
            set { _computers = value; OnPropertyChanged(); }
        }

        public async void Deal_click(object sender, EventArgs e)
        {
            int MyAantal = Game.MyAantal;
            if (MyAantal <= User.Tokens && MyAantal > 0)
            {
                bool result = await ApiWrapper.UpdateTokens(MyAantal * -1, _sender);
                if (result)
                {
                    Game.Deal();
                    await ApiWrapper.GetUserInfo();
                    ComputerGame.Computerstart();

                    int Player = Game.Players[0].Score;
                    if (Player == 21)
                    {
                        await DisplayAlert($"BlackJack", "Blackjack!!!!!!", "OK");
                        Game.Stand21();
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        ComputerGame.ComputerDeal(Player);
                        Endresult();
                    }
                }
                else
                {
                    await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
            }
            else
            {
                await DisplayAlert("Tokens","Te weinig tokens om in te zetten!","OK");
            }
            Account();
        }

        private void Hit_Click(object sender, EventArgs e)
        {
            Game.Hits();
            int Player = Game.Players[0].Score;

            if (Player > 21)
            {
                Game.Stands();
                Endresult();
            }
        }

        private async void Dubbelen_Click(object sender, EventArgs e)
        {
            bool ingelogd = await Checkingelogd();
            if (ingelogd)
            {
                int MyAantal = Game.MyAantal;
                if (MyAantal <= User.Tokens)
                {
                    Game.Dubbelen();
                    Dubbelentrue = true;
                    await ApiWrapper.GetUserInfo();

                    int Player = Game.Players[0].Score;
                    if (Player > 21)
                    {
                        ComputerGame.ComputerDeal(Player);
                        Game.Stands();
                        Endresult();
                    }
                }
                else
                {
                    await DisplayAlert("Tokens", "Te weinig tokens om in te zetten!", "OK");
                }
            }
            Account();
        }

        private async void Splitten_Click(object sender, EventArgs e)
        {
            bool ingelogd = await Checkingelogd();
            if (ingelogd)
            {
                Splitfunction = true;
                Game.Splitte();
                await ApiWrapper.GetUserInfo();
            }
            Account();
        }

        private async void Stand_Click(object sender, EventArgs e)
        {
            bool ingelogd = await Checkingelogd();
            if (ingelogd)
            {
                Game.Stands();
                Endresult();
                Account();
            }
        }


        private void Endresult()
        {
            try
            {
                if (Splitfunction == false && Deckswitch == false)
                {
                    Computersbet();
                    int bot = ComputerGame.Computer[0].ScoreC;
                    int Player = Game.Players[0].Score;

                    if (Player == 21 && bot < 21)
                    {
                        //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                        Game.Gamewin();
                        Endresults();
                    }
                    else if (bot > 21 && Player > 21)
                    {
                        //DisplayAlert($"BlackJack", "Allebei verloren!", "OK");
                        Endresults();
                    }
                    else if (bot > 21 && Player <= 21)
                    {
                        //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                        Game.Gamewin();
                        Endresults();
                    }
                    else if (Player > bot && Player <= 21)
                    {
                        //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                        Game.Gamewin();
                        Endresults();
                    }
                    else if (Player > 21 && bot <= 21)
                    {
                        //DisplayAlert($"BlackJack", "Je hebt verloren!", "OK");
                        Endresults();
                    }
                    else if (bot > Player && bot <= 21)
                    {
                        //DisplayAlert($"BlackJack", "Je hebt verloren!", "OK");
                        Endresults();
                    }
                    else if (bot == Player)
                    {
                        //DisplayAlert($"BlackJack", "Gelijkspel!", "OK");
                        Game.Gamedraw();
                        Endresults();
                    }
                    else
                    {
                        //DisplayAlert($"BlackJack", "Fout!", "OK");
                        Endresults();
                    }
                }
                else if (Splitfunction == true && Deckswitch == false)
                {
                    Deckswitch = true;
                    Game.Splitfunction2();
                }
                else if (Deckswitch == true && Splitfunction == true)
                {
                    Computersbet();
                    //DisplayAlert($"BlackJack", "computers beurt", "OK");
                    Resultrl();
                }
            }
            catch
            {
                Account();
            }
        }

        public void Resultrl()
        {
            int bot = ComputerGame.Computer[0].ScoreC;
            int Player = int.Parse(Rightscore.Text);
            int Player2 = int.Parse(Leftscore.Text);

            if (Player2 == 21 && bot < 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                Game.Gamewin();
                Endresults();
            }
            else if (bot > 21 && Player2 > 21)
            {
                //DisplayAlert($"BlackJack", "Allebei verloren!", "OK");
                Endresults();
            }
            else if (bot > 21 && Player2 <= 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                Game.Gamewin();
                Endresults();
            }
            else if (Player2 > bot && Player2 <= 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                Game.Gamewin();
                Endresults();
            }
            else if (Player2 > 21 && bot <= 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt verloren!", "OK");
                Endresults();
            }
            else if (bot > Player2 && bot <= 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt verloren!", "OK");
                Endresults();
            }
            else if (bot == Player2)
            {
                //DisplayAlert($"BlackJack", "Gelijkspel!", "OK");
                Game.Gamedraw();
                Endresults();
            }
            else
            {
                //DisplayAlert($"BlackJack", "Fout!", "OK");
                Endresults();
            }


            if (Player == 21 && bot < 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                Game.Gamewin2();
                Endresults();
            }
            else if (bot > 21 && Player > 21)
            {
                //DisplayAlert($"BlackJack", "Allebei verloren!", "OK");
                Endresults();
            }
            else if (bot > 21 && Player <= 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                Game.Gamewin2();
                Endresults();
            }
            else if (Player > bot && Player <= 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt gewonnen!", "OK");
                Game.Gamewin2();
                Endresults();
            }
            else if (Player > 21 && bot <= 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt verloren!", "OK");
                Endresults();
            }
            else if (bot > Player && bot <= 21)
            {
                //DisplayAlert($"BlackJack", "Je hebt verloren!", "OK");
                Endresults();
            }
            else if (bot == Player)
            {
                //DisplayAlert($"BlackJack", "Gelijkspel!", "OK");
                Game.Gamedraw2();
                Endresults();
            }
            else
            {
                //DisplayAlert($"BlackJack", "Fout!", "OK");
                Endresults();
            }
        }

        public void Computersbet()
        {
            int Player = Game.Players[0].Score;
            ComputerGame.ComputerDeal(Player);
        }

        private int _aantalonthouden;

        public int Aantalonthouden
        {
            get { return _aantalonthouden; }
            set { _aantalonthouden = value; OnPropertyChanged(); }
        }


        public async void Endresults()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            Game.Blackjackwindow();

            if (Dubbelentrue)
            {
                Aantalonthouden = Game.MyAantal / 2;
            }
            else if (Splitfunction)
            {
                Aantalonthouden = Game.MyAantal / 2;
            }
            else
            {
                Aantalonthouden = Game.MyAantal;
            }

            Game.Gameclear();
            ComputerGame.GameclearComputer();
            await ApiWrapper.GetUserInfo();
            Account();
            BlackjackWindowRestart();
            Dubbelentrue = false;
            Splitfunction = false;
            Deckswitch = false;
            //Einde game
        }

        private async Task<bool> Checkingelogd()
        {
            //bool result = await ApiWrapper.CheckLogin();

            //if (!result)
            //{
            //    StonksCasino.Properties.Settings.Default.Username = "";
            //    StonksCasino.Properties.Settings.Default.Password = "";
            //    StonksCasino.Properties.Settings.Default.Save();
            //    User.Username = "";
            //    User.Tokens = 0;

            //    MainWindow window = new MainWindow();

            //    MessageBox.Show("Er is door iemand anders ingelogd op het account waar u momenteel op speelt. Hierdoor wordt u uitgelogd");
            //    this.Close();
            //    window.Show();

            //    return false;
            //}
            return true;
        }
    }
}
