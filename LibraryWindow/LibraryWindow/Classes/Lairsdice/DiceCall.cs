using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryWindow.Classes.Lairsdice
{
    public class DiceCall : PropertyChangedBase
    {
        private int _diceAmount;

        public int DiceAmount
        {
            get { return _diceAmount; }
            set { _diceAmount = value; }
        }

        private int _diceType;

        public int DiceType
        {
            get { return _diceType; }
            set { _diceType = value; }
        }


    }
}
