using LibraryWindow.classes.Api;
using LibraryWindow.Classes.Main;
using LibraryWindow.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LibraryWindow.Classes.BlackJack
{
    public class BlackJacken : PropertyChange
    {
        private const string _sender = "Blackjack";

        public bool SplitGrid
        {
            get { return _splitted ? true : false; }
        }

        public bool NoSplitGrid
        {
            get { return _splitted ? false : true; }
        }

        private bool _splitted = false;

        public bool Splitted
        {
            get { return _splitted; }
            set { _splitted = value; OnPropertyChanged("SplitGrid"); OnPropertyChanged("NoSplitGrid"); }
        }

        private int _aantal;

        public int MyAantal
        {
            get { return _aantal; }
            set { _aantal = value; OnPropertyChanged(); }
        }

        private int _PlayerSplit;

        public int MyPlayerSplit
        {
            get { return _PlayerSplit; }
            set { _PlayerSplit = value; OnPropertyChanged(); }
        }

        private bool _tokendrop = true;

        public bool Tokendrop
        {
            get { return _tokendrop; }
            set { _tokendrop = value; OnPropertyChanged(); }
        }

        private bool _dubbel;

        public bool Dubbel
        {
            get { return _dubbel; }
            set { _dubbel = value; OnPropertyChanged(); }
        }

        private bool _splitten;

        public bool Splitten
        {
            get { return _splitten; }
            set { _splitten = value; OnPropertyChanged(); }
        }

        private bool _stand;

        public bool Stand
        {
            get { return _stand; }
            set { _stand = value; OnPropertyChanged(); }
        }

        private bool _hit;

        public bool Hit
        {
            get { return _hit; }
            set { _hit = value; OnPropertyChanged(); }
        }

        private bool _deals;

        public bool Deals
        {
            get { return _deals; }
            set { _deals = value; OnPropertyChanged(); }
        }

        private bool _standing;

        public bool Standing
        {
            get { return _standing; }
            set { _standing = value; OnPropertyChanged(); }
        }

        private bool _splitstands = false;

        public bool Splitstands
        {
            get { return _splitstands; }
            set { _splitstands = value; OnPropertyChanged(); }
        }


        private BlackjackDeck deck = new BlackjackDeck();

        private List<BlackjackPlayer> _players;

        List<Main.CardBlackjack> cards = new List<Main.CardBlackjack>();

        public List<BlackjackPlayer> Players
        {
            get { return _players; }
            set { _players = value; OnPropertyChanged(); }
        }

        public BlackJacken()
        {
            Players = new List<BlackjackPlayer>();
            Players.Add(new BlackjackPlayer());
        }

        public void Deal()
        {
            try
            {
                if (MyAantal > 0)
                {
                    Deals = false;
                    SetPlayerHand(Players[0]);
                    Tokendrop = false;
                    Dubbel = true;
                    Splitten = CheckSplit();
                    Standing = true;
                    Hit = true;

                    string MyAantalString = MyAantal.ToString();
                    //App.Current.MainPage.DisplayAlert($"Ingezet", MyAantalString, "OK");
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Te weinig tokens", "U moet minimaal 1 token inzetten om te kunnen spelen!", "OK");
                }
            }
            catch
            {
                App.Current.MainPage.DisplayAlert("Te weinig tokens", "U moet minimaal 1 token inzetten om te kunnen spelen!", "OK");
            }
        }

        public void SetPlayerHand(BlackjackPlayer player)
        {
            cards.Add(deck.DrawCard());
            cards.Add(deck.DrawCard());
            player.SetHand(cards);

            //MyPlayerSplit = 1;
        }

        public bool CheckSplit()
        {
            return cards[0].Value == cards[1].Value;
        }

        public void Hits()
        {
            if (Splitted == false)
            {
                Splitten = false;
                Hit = true;
                Dubbel = false;
                Players[0].AddCard(deck.DrawCard());
            }
            else if (Splitted == true)
            {
                Hit = true;
                Dubbel = false;
                Players[0].AddCard(deck.DrawCard());
            }
            //App.Current.MainPage.DisplayAlert($"Hit", "Er is gehit", "OK");
        }

        public async void Dubbelen()
        {
            try
            {
                await ApiWrapper.UpdateTokens(-MyAantal, _sender);
                MyAantal = MyAantal * 2;
                Dubbel = false;
                Hit = false;
                Players[0].AddCard(deck.DrawCard());
                //App.Current.MainPage.DisplayAlert($"Dubbel", "Het aantal is verdubbeld en u pakt een kaart!", "OK");
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        public async void Splitte()
        {
            try
            {
            await ApiWrapper.UpdateTokens(-MyAantal, _sender);
            Splitten = false;
            MyAantal = MyAantal * 2;
            Players[0].Split(deck.DrawCard(), deck.DrawCard());
            Splitted = true;
            Players[0].Changescore2();
            Players[0].Update();
                //App.Current.MainPage.DisplayAlert($"Splitten", "Er wordt gesplit", "OK");
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        public void Splitfunction2()
        {
            Splitstands = true;
        }

        public void Stands()
        {
            if (Splitted == true)
            {
                if (Splitstands == false)
                {
                    Splitstand();
                    Players[0].SplitRightHand();
                    Players[0].Changescore();
                    Players[0].Update2();
                }
                else if (Splitstands == true)
                {
                    Splitstand();
                    Standing = false;
                    Hit = false;
                    Deals = true;
                    Tokendrop = true;
                    Splitstands = false;
                }
            }
            else
            {
                Splitstand();
                //Deals = true;
                Standing = false;
                Tokendrop = true;
                Hit = false;
                //App.Current.MainPage.DisplayAlert($"Stand", "Er wordt gestand, de computer is nu aan de beurt!", "OK");
            }
        }

        public void Blackjackcomputerens()
        {
            Hit = false;
            Dubbel = false;
            Splitten = false;
        }

        public void Stand21()
        {
            Hit = false;
            Standing = false;
            Dubbel = false;
        }

        public void Splitstand()
        {
            Splitten = false;
            Dubbel = false;
        }

        public async void Gamewin()
        {
            try
            {
                int a = MyAantal * 2;
                await ApiWrapper.UpdateTokens(MyAantal * 2, _sender);
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        public async void Gamedraw()
        {
            try
            {
            await ApiWrapper.UpdateTokens(MyAantal, _sender);
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        public async void Gamewin2()
        {
            try
            {
                int a = MyAantal * 2;
                await ApiWrapper.UpdateTokens(a, _sender);
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        public async void Gamedraw2()
        {
            try
            {
                await ApiWrapper.UpdateTokens(MyAantal, _sender);
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }

}

        public void Gameclear()
        {
            Players[0].GameOver();
        }

        public void Blackjackwindow()
        {
            Deals = true;
            //Tokendrop = true;
            Splitten = false;
            Dubbel = false;
            Standing = false;
            Hit = false;
        }
    }
}
