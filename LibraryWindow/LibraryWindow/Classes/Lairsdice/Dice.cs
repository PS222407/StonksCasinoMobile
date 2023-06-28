using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LibraryWindow.Classes.Lairsdice
{
   public class Dice : PropertyChangedBase
    {
        private Random random;

        private int _roll = 0;

        public int Roll
        {
            get
            {
                return _roll;
            }
            set { _roll = value; OnPropertyChanged("ImgSource"); OnPropertyChanged(); }
        }

        public ImageSource ImgSource                             
        {
            get { return ImageSource.FromResource($"LibraryWindow.Images.Lairsdice.Dice.{Roll}.png"); }
        }

        public Dice(Random random)
        {
            this.random = random;
        }

        public void RollDice()
        {
            Roll = random.Next(1, 7);
        }

    }
}
