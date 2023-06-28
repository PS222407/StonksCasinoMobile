using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LibraryWindow.Classes.Lairsdice
{
    public class Player : PropertyChangedBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private bool _activeCall = false;

        public bool ActiveCall
        {
            get { return _activeCall; }
            set { _activeCall = value; OnPropertyChanged(); }
        }

        private bool _activeLiar = false;

        public bool ActiveLiar
        {
            get { return _activeLiar; }
            set { _activeLiar = value; OnPropertyChanged(); }
        }
        private bool _active = false;

        public bool Active
        {
            get { return _active; }
            set { _active = value; OnPropertyChanged("NameColor"); }
        }

        public string NameColor 
        {
            get
            {
                if (Active)
                {
                    return "Red";
                }
                else
                {
                    return "White";
                }
            }
        }
  




        public Throw MyThrow { get; set; }

        public bool Out { get; set; } = false;

        private double _cupOpacity = 1;

        public double CupOpacity
        {
            get { return _cupOpacity; }
            set { _cupOpacity = value; OnPropertyChanged(); }
        }



    }
}
