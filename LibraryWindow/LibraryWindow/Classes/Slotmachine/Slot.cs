using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LibraryWindow.Classes.Main;

namespace LibraryWindow.Classes.Slotmachine
{
    public class Slot : PropertyChange
    {
        public ImageSource ImageURL
        {
            get { return ImageSource.FromResource($"LibraryWindow.Images.Slotmachine.{Name}.png"); }
        }
        //ImageSource.FromResource("SlotMachine.Image.background.png");

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("ImageURL"); }
        }
    }
}
