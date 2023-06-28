using LibraryWindow.classes.Main;
using LibraryWindow.Classes.enums.card;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace LibraryWindow.Classes.Main
{
	public class CardBlackjack : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public CardBlackjack()
        {
			
        }

		private CardType _type;

		public CardType Type
		{
			get { return _type; }
			set { _type = value; OnPropertyChanged("ImageURL"); OnPropertyChanged("ActiveURL"); }
		}

		private BlackcardValue _value;

		public BlackcardValue Value
		{
			get { return _value; }
			set { _value = value; OnPropertyChanged("ImageURL"); OnPropertyChanged("ActiveURL"); }
		}

		private bool _turned;

		public bool Turned
		{
			get { return _turned; }
			set { _turned = value; OnPropertyChanged(); OnPropertyChanged("ActiveURL"); }
		}

		public ImageSource ActiveURL
		{
			get
			{
				if (Turned)
				{
					return BackURL;
				}
				else
				{
					return ImageURL;
				}
			}
		}

		public string SelectedCardskin
		{
			get { return User.SelectedCardskin; }
		}

		private CardBackColor _backColor;

		public CardBackColor BackColor
		{
			get { return _backColor; }
			set { _backColor = value; OnPropertyChanged("BackURL"); OnPropertyChanged("ActiveURL"); }
		}

		public ImageSource ImageURL
		{
			get { return ImageSource.FromResource($"LibraryWindow.Images.Cards.{SelectedCardskin}.{Value}{Type}.png"); }
		}

		public ImageSource BackURL
		{
			get
			{
				if (BackColor == CardBackColor.Blue)
				{
					return ImageSource.FromResource($"LibraryWindow.Images.Cards.{SelectedCardskin}.BackBlue.png");
				}
				else
				{
					return ImageSource.FromResource($"LibraryWindow.Images.Cards.{SelectedCardskin}.BackRed.png");
				}
			}
		}

		public CardBlackjack(CardType cardType, BlackcardValue cardValue, CardBackColor cardBackColor)
		{
			Type = cardType;
			Value = cardValue;
			BackColor = cardBackColor;
			Turned = false;
			OnPropertyChanged("ImageURL");
		}

		public CardBlackjack(CardType cardType, BlackcardValue cardValue, CardBackColor cardBackColor, bool turned)
		{
			Type = cardType;
			Value = cardValue;
			BackColor = cardBackColor;
			Turned = turned;
			OnPropertyChanged("ImageURL");
		}
	}
}
