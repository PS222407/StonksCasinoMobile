using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryWindow.Classes.Lairsdice
{
    public class Throw : PropertyChangedBase
    {
        private Random random;

        private List<Dice> _dices = new List<Dice>();  

        public List<Dice> Dices
        {
            get { return _dices; }
            set { _dices = value; OnPropertyChanged(); }
        }

        public Throw(Random random)
        {
            this.random = random;
            Dices.Add(new Dice(random));
            Dices.Add(new Dice(random));
            Dices.Add(new Dice(random));
            Dices.Add(new Dice(random));
            Dices.Add(new Dice(random));
        }

        public void ThrowDice()
        {
            foreach (Dice dice in Dices)
            {
                dice.RollDice();
            }
        }
    }
}
