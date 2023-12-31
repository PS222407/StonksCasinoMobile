﻿using LibraryWindow.Classes.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryWindow.Classes.BlackJack
{
    public class Computers : PropertyChange
    {
        private BlackJacken _blackjackhit;

        public BlackJacken Myblackjackhit
        {
            get { return _blackjackhit; }
            set { _blackjackhit = value; }
        }

        private int _Computervalue;

        public int MyComputervalue
        {
            get { return _Computervalue; }
            set { _Computervalue = value; OnPropertyChanged(); }
        }

        private BlackjackDeckComputer deck = new BlackjackDeckComputer();

        private List<BlackjackComputer> _computer;

        List<CardBlackjack> cards = new List<CardBlackjack>();

        public List<BlackjackComputer> Computer
        {
            get { return _computer; }
            set { _computer = value; OnPropertyChanged(); }
        }

        public Computers()
        {
            Computer = new List<BlackjackComputer>();
            Computer.Add(new BlackjackComputer());
        }

        private BlackJacken _gamechanger = new BlackJacken();

        public BlackJacken Gamechanger
        {
            get { return _gamechanger; }
            set { _gamechanger = value; OnPropertyChanged(); }
        }


        public async void Computerstart()
        {
            SetComputerHand(Computer[0]);

            int infoace = Computer[0].GameScoreFake;

            if (infoace == 11)
            {
                var action = await App.Current.MainPage.DisplayAlert($"Verzekering", "Je kunt nu verzekeren om niet je hele inzet te verliezen, maar je als wint krijg je alleen je inzet terug!", "Yes", "No");
                if (action)
                {
                    await App.Current.MainPage.DisplayAlert($"Verzekering", "Je hebt verzekerd", "ok");
                    if (Computer[0].GameScore == 21)
                    {
                        Gamechanger.Blackjackcomputerens();
                        cards[0].Turned = false;
                        cards[1].Turned = false;
                        Computer[0].Secondcard = true;
                    }
                }
                else
                {
                    if (Computer[0].GameScore == 21)
                    {
                        Gamechanger.Blackjackcomputerens();
                        cards[0].Turned = false;
                        cards[1].Turned = false;
                        Computer[0].Secondcard = true;
                    }
                }
            }
        }

        public void ComputerDeal(int player)
        {
            cards[0].Turned = false;
            cards[1].Turned = false;
            Computer[0].Test();
            Computer[0].Secondcard = true;
            Computerhit(player);
        }

        public void Computerhit(int playervalue)
        {

            int Player = playervalue;
            int Bot = Computer[0].ScoreC;

            if (Bot < 17)
            {
                Computer[0].AddCard(deck.DrawCard());
                Computerhit(playervalue);
            }
            else if (Bot < 17 && playervalue > Bot && playervalue < 21)
            {
                Computer[0].AddCard(deck.DrawCard());
                Computerhit(playervalue);

            }

        }

        public void GameclearComputer()
        {
            Computer[0].GameOver();
            Computer[0].Secondcard = false;
        }

        private CardBlackjack _turncard = new CardBlackjack();

        public CardBlackjack TurnCard
        {
            get { return _turncard; }
            set { _turncard = value; OnPropertyChanged(); }
        }


        public void SetComputerHand(BlackjackComputer computer)
        {
            CardBlackjack card2 = deck.DrawCard();
            CardBlackjack card = deck.DrawCard();

            card2.Turned = true;
            cards.Add(card);
            cards.Add(card2);

            computer.SetHandC(cards);
        }
    }
}
