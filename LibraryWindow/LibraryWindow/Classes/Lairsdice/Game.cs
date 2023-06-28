using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using LibraryWindow.classes.Api;

namespace LibraryWindow.Classes.Lairsdice
{
    public class Game : PropertyChangedBase
    {
        private Random random = new Random();


        private string _gameLogCall = "Klik op start om te beginnen";

        public string GameLogCall
        {
            get { return _gameLogCall; }
            set { _gameLogCall = value; OnPropertyChanged(); }
        }

        private string _gameLogSubtext;

        public string GameLogSubtext
        {
            get { return _gameLogSubtext; }
            set { _gameLogSubtext = value; OnPropertyChanged(); }
        }



        public int ActivePlayers 
        {
            get
            {
                int result = 0;
                foreach (Player player in Players)
                {
                    if (!player.Out)
                    {
                        result++;
                    }
                }
                return result;
            }
        }

        private ObservableCollection<Player> _players = new ObservableCollection<Player>();

        public ObservableCollection<Player> Players
        {
            get { return _players; }
            set { _players = value; OnPropertyChanged(); }
        }

        private DiceCall _activeCall = new DiceCall() { DiceAmount = 0, DiceType = 0};

        public DiceCall ActiveCall
        {
            get { return _activeCall; }
            set { _activeCall = value; }
        }

        private int _turn;

        public int Turn
        {
            get { return _turn; }
            set { _turn = value; OnPropertyChanged("ActivePlayer"); }
        }

        private bool _startBtn = true;

        public bool Startbtn
        {
            get { return _startBtn; }
            set { _startBtn = value; OnPropertyChanged(); }
        }

        private int _bet;


        public Player ActivePlayer 
        {
            get { return Players[_turn % 4]; }
        }

        public Game(string username, int bet)
        {
            _bet = bet * 3;
            Players.Add(new Player() { Name = username, MyThrow = new Throw(random) });
            Players.Add(new ComPlayer() { Name = "Kees", MyThrow = new Throw(random), MyRandom = random });
            Players.Add(new ComPlayer() { Name = "Josephine", MyThrow = new Throw(random), MyRandom = random });
            Players.Add(new ComPlayer() { Name = "Jan", MyThrow = new Throw(random), MyRandom = random });
        }


        public async void NextTurn()
        {
            ActivePlayer.Active = false;
            Turn++;
            Checkactive("+");
            ActivePlayer.Active = true;
            if (ActivePlayer.GetType() == typeof(ComPlayer))
            {
                Players[0].ActiveCall = false;
                Players[0].ActiveLiar = false;
                await Task.Delay(2000);
                bool liar = false;           
                    ((ComPlayer)ActivePlayer).PlayTurn(ActiveCall, ActivePlayers, out liar);
               
                if (liar)
                {
                    bool liarCheck = CheckLiar();
                    GameLogCall = $"{ActivePlayer.Name} called:";
                    GameLogSubtext = "liar";
                    await Task.Delay(2000);



                    if (liarCheck)
                    {
                        
                        
                        GameLogCall = $"{ActivePlayer.Name} was correct";
                        Turn--;
                        Checkactive("-");
                        GameLogSubtext = $"{ActivePlayer.Name} is er uit";
                        await Task.Delay(4000);

                        ActivePlayer.Out = true;
                        ActivePlayer.CupOpacity = 0;
                        if (ActivePlayers > 1)
                        {
                            foreach (Player player in Players)
                        {
                            if (player.Out == false)
                            {
                                player.CupOpacity = 1;
                  
                            }
                            else
                            {
                                foreach (Dice dice in player.MyThrow.Dices)
                                {
                                    dice.Roll = 0;
                                }

                            }
                        }

                            ActiveCall.DiceAmount = 0;
                            ActiveCall.DiceType = 0;
                            await Task.Delay(2000);


                            foreach (Player player in Players)
                            {
                                if (!player.Out)
                                {
                                    player.MyThrow.ThrowDice();
                                }
                            }
                            if (!Players[0].Out)
                            {
                                Players[0].CupOpacity = 0.3;
                                NextTurn();
                            }
                            else
                            {
                                ResetGame();
                            }
                            

                        }
                        else
                        {
                            ResetGame();
                        }

                    }
                    else
                    {
                        GameLogCall = $"{ActivePlayer.Name} had het fout";
                        GameLogSubtext = $"{ActivePlayer.Name} ligt er uit";
                        await Task.Delay(2000);

                        ActivePlayer.Out = true;
                        ActivePlayer.Active = false;
                        ActivePlayer.CupOpacity = 0;
                        if (ActivePlayers > 1)
                        {
                            foreach (Player player in Players)
                        {
                            if (player.Out == false)
                            {
                                player.CupOpacity = 1;

                            }
                            else
                            {
                                foreach (Dice dice in player.MyThrow.Dices)
                                {
                                    dice.Roll = 0;
                                }

                            }
                        }


                            
                            ActiveCall.DiceAmount = 0;
                            ActiveCall.DiceType = 0;
                            await Task.Delay(2000);

                            foreach (Player player in Players)
                            {
                                if (!player.Out)
                                {
                                    player.MyThrow.ThrowDice();
                                }
                            }
                            if (!Players[0].Out)
                            {
                                Players[0].CupOpacity = 0.3;
                                NextTurn();
                            }
                            else
                            {
                                ResetGame();
                            }
                           
                        }
                        else
                        {
                            ResetGame();
                        }
                    }
                    
                }
                else
                {
                    GameLogCall = $"{ActivePlayer.Name} zegt:";
                    GameLogSubtext = $"{ActiveCall.DiceAmount} X {ActiveCall.DiceType}";
                    NextTurn();

                }
            }
            else
            {
                
                Players[0].ActiveCall = true;
                Players[0].ActiveLiar = true;

            }
        }

        private void Checkactive(string plus)
        {
            switch (plus)
            {
                case "+":
                    while (ActivePlayer.Out)
                    {
                        Turn++;
                    }
                    break;
                case "-":
                    while (ActivePlayer.Out)
                    {
                        Turn--;
                    }
                   
                    break;
               
            }
           
        }
        private async void ResetGame()
        {
            if (Players[0].Out)
            {
                GameLogCall = "Je ligt er uit:";
                GameLogSubtext = "klik op start om opnieuw te beginnen.";
            }
            else
            {
                GameLogCall = $"Gefeliciteerd je hebt {_bet} tokens gewonnen";
                GameLogSubtext = "klik op start om opnieuw te beginnen.";
                await ApiWrapper.UpdateTokens(_bet, "Mobile LiarsDice, Uitbetaling");
                await ApiWrapper.GetUserInfo();
            }

            Startbtn = true;
            Players[0].ActiveCall = false;
            Players[0].ActiveLiar = false;
        }
        
        public void StartGame()
        {
            _turn = random.Next(0,3);
            ActivePlayer.Active = true;
       
            if (ActivePlayer.GetType() == typeof(ComPlayer))
            {
                
                NextTurn();
            }
            else
            {
                Players[0].ActiveCall = true;
                GameLogCall = "U bent aan de beurt";

            }
        }
        public void SetActiveCall(int DiceAmount, int DiceType)
        {
            ActiveCall.DiceAmount = DiceAmount;
            ActiveCall.DiceType = DiceType;
        }
        public bool CheckLiar()
        {
            int PresentDice = 0;
            foreach(Player player in Players)
            {
                if (player.Out == false)
                {
                    player.CupOpacity = 0.3;
                    foreach (Dice dice in player.MyThrow.Dices)
                    {
                        if(dice.Roll == ActiveCall.DiceType)
                        {
                            PresentDice++;
                        }
                    }
                }
            }
            if (ActiveCall.DiceAmount > PresentDice)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async void Playerliar(bool liar)
        {
            if (liar)
            {


                GameLogCall = $"{ActivePlayer.Name} was correct";
                Turn--;
                Checkactive("-");
                GameLogSubtext = $"{ActivePlayer.Name} is er uit";
                await Task.Delay(4000);
                ActivePlayer.Out = true;
                ActivePlayer.CupOpacity = 0;
                if (ActivePlayers > 1)
                {
                    foreach (Player player in Players)
                    {
                        if (player.Out == false)
                        {
                            player.CupOpacity = 1;

                        }
                        else
                        {
                            foreach (Dice dice in player.MyThrow.Dices)
                            {
                                dice.Roll = 0;
                            }

                        }
                    }


                    ActiveCall.DiceAmount = 0;
                    ActiveCall.DiceType = 0;
                    await Task.Delay(2000);

                    foreach (Player player in Players)
                    {
                        if (!player.Out)
                        {
                            player.MyThrow.ThrowDice();
                        }
                    }
                    if (!Players[0].Out)
                    {
                        Players[0].CupOpacity = 0.3;
                        NextTurn();
                    }
                    else
                    {
                        ResetGame();
                    }


                }
                else
                {
                    ResetGame();
                }

            }
            else
            {
                GameLogCall = $"{ActivePlayer.Name} had het fout";
                GameLogSubtext = $"{ActivePlayer.Name} ligt er uit";
                await Task.Delay(4000);
                ActivePlayer.Out = true;
                ActivePlayer.Active = false;
                ActivePlayer.CupOpacity = 0;
                if (ActivePlayers > 0)
                {
                    foreach (Player player in Players)
                    {
                        if (player.Out == false)
                        {
                            player.CupOpacity = 1;

                        }
                        else
                        {
                            foreach (Dice dice in player.MyThrow.Dices)
                            {
                                dice.Roll = 0;
                            }

                        }
                    }



                    ActiveCall.DiceAmount = 0;
                    ActiveCall.DiceType = 0;
                    await Task.Delay(2000);

                    foreach (Player player in Players)
                    {
                        if (!player.Out)
                        {
                            player.MyThrow.ThrowDice();
                        }
                    }
                    if (!Players[0].Out)
                    {
                        Players[0].CupOpacity = 0.3;
                        NextTurn();
                    }
                    else
                    {
                        ResetGame();
                    }

                }
                else
                {
                    ResetGame();
                }
            }

        }
    }


    }

