using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryWindow.Classes.Lairsdice
{
    public class ComPlayer : Player
    {
        private Random _random;

        public Random MyRandom
        {
            get { return _random; }
            set { _random = value; }
        }


        public void PlayTurn(DiceCall diceCall, int playersleft, out bool liar)
        {
            double liarChance = 0;
            double AdjustedDice = diceCall.DiceAmount - CheckOwn(diceCall.DiceType);
            if (AdjustedDice > 0)
            {
                liarChance = CalculateChange((playersleft - 1) * 5, AdjustedDice);
                
            }
            int diceamount = 0;
            int dicetype = 0;
            double callchance = Callchance(diceCall, playersleft, out diceamount, out dicetype);
            if (!(liarChance == 0))
            {
                liarChance = 70 - liarChance;
            }
           
            if (liarChance > callchance)
            {
                liar = true;
            }
            else
            {
                diceCall.DiceAmount = diceamount;
                diceCall.DiceType = dicetype;
                liar = false;
            }

           }
        

        private double Callchance(DiceCall diceCall, int playersleft, out int diceamount, out int dicetype)
        {
            List<Dice> orderedDice = MyThrow.Dices.OrderBy(x => x.Roll).ToList();
            
            List<List<int>> DicePairs = new List<List<int>>();
            List<int> DicePair = new List<int>();
            foreach (Dice dice in orderedDice)
            {
                if (dice.Roll >= diceCall.DiceType)
                {
                    if (DicePair.Count == 0 || dice.Roll == DicePair[0])
                    {
                        DicePair.Add(dice.Roll);
                    }
                    else
                    {
                        DicePairs.Add(DicePair);
                        DicePair = new List<int>();
                        DicePair.Add(dice.Roll);
                    }
                }
            }
            if (DicePairs.Count == 0)
            {
                int calleddice = diceCall.DiceAmount + 1;
                double chance = CalculateChange((playersleft - 1) * 5, calleddice);
                chance += 20;
                diceamount = calleddice;
                dicetype = diceCall.DiceType;
                return chance;

            }
            List<int> HighestDicePair = new List<int>();
            foreach (List<int> dicepair in DicePairs)
            {
                if (dicepair.Count > HighestDicePair.Count || (dicepair.Count == HighestDicePair.Count && dicepair[0] > HighestDicePair[0]))
                {
                    HighestDicePair = dicepair;
                }
            }
            int CallDelta = HighestDicePair.Count - diceCall.DiceAmount;
          
            if (CallDelta > 0)
            {
                diceamount = HighestDicePair.Count;
                dicetype = HighestDicePair[0];
                return 100;

            }
            else
            {
                int CallAmount;
                if (HighestDicePair[0] == diceCall.DiceType)
                {
                    CallAmount = HighestDicePair.Count + Math.Abs(CallDelta) + 1;
                }
                else
                {
                    CallAmount = HighestDicePair.Count + Math.Abs(CallDelta);
                }
                double chance = CalculateChange((playersleft - 1) * 5, CallAmount - HighestDicePair.Count );
                diceamount = CallAmount;
                dicetype = HighestDicePair[0];
                return chance;
            }
        

            }

       
        private double CalculateChange(double d, double n)
        {
            double result = 0;
            for (double i = n; i <= d; i++)
            {
                result += (Faculty(d) / (Faculty(i) * Faculty(d - i))) * Math.Pow((1.0 / 6.0), i) * Math.Pow((5.0 / 6.0), d - i);
            }
            return result * 100;
        }

        private double Faculty(double n)
        {
            double result = 1;
            for (double i = n; i > 0; i--)
            {
                result *= i;
            }
            return result;
        }
        private double CheckOwn(int diceType)
        {
            double PresentDice = 0;

            foreach (Dice dice in MyThrow.Dices)
            {
                if (dice.Roll == diceType)
                {
                    PresentDice++;
                }
            }

            return PresentDice;
            
        }

    }
}
