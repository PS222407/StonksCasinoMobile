using LibraryWindow.classes.Api;
using LibraryWindow.classes.Main;
using LibraryWindow.Classes.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibraryWindow.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WheelOfFortune : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        bool blnShouldStay = false;
        protected override bool OnBackButtonPressed()
        {
            if (blnShouldStay || WinAmount > 0)
            {
                if (blnShouldStay == true)
                {
                    App.Current.MainPage.DisplayAlert("Let op!", "Wacht tot dat het rad is uitgedraaid.", "OK");
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Let op!", "Vergeet niet om op stop te drukken om je pot veilig te stellen op je account.", "OK");
                }
                return true;
            }
            else
            {
                base.OnBackButtonPressed();
                return false;
            }
        }

        private const string _sender = "WheelOfFortune";

        public string Username
        {
            get { return User.Username; }
        }

        public int Tokens
        {
            get { return User.Tokens; }
        }

        private async void Account()
        {
            try
            {
                bool result = await ApiWrapper.GetUserInfo();
                OnPropertyChanged("Username");
                OnPropertyChanged("Tokens");
                if (!result)
                {
                    await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
            }
            catch
            {
                await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        private async void Uitloggen_Pressed(object sender, EventArgs e)
        {
            Preferences.Remove("user_name");
            Preferences.Remove("pass_word");
            Preferences.Remove("remember");

            try
            {
                bool logout = await User.LogoutAsync();
                if (logout)
                {
                    await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
            }
            catch
            {
                await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        [Obsolete]
        private void Store_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/winkel"));
        }

        [Obsolete]
        private void Deposit_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/account-info"));
        }


        private double _myAngle;
        public double MyAngle
        {
            get { return _myAngle; }
            set { _myAngle = value; OnPropertyChanged(); }
        }

        private double _myStartPoint = 7.5;
        public double MyStartPoint
        {
            get { return _myStartPoint; }
            set { _myStartPoint = value; OnPropertyChanged(); }
        }

        private int _winAmount;
        public int WinAmount
        {
            get { return _winAmount; }
            set { _winAmount = value; OnPropertyChanged(); OnPropertyChanged("Tokens"); }
        }

        private string _buttonSwitch = "True";
        public string ButtonSwitch
        {
            get { return _buttonSwitch; }
            set { _buttonSwitch = value; OnPropertyChanged(); }
        }

        private double _cost;
        public double Cost
        {
            get { return _cost; }
            set { _cost = value; OnPropertyChanged(); }
        }

        private int _gameCount;
        public int GameCount
        {
            get { return _gameCount; }
            set { _gameCount = value; OnPropertyChanged(); }
        }

        private string _wheelImg = "wheel.png";
        public string WheelImgString
        {
            get { return _wheelImg; }
            set { _wheelImg = value; OnPropertyChanged("WheelImg"); }
        }

        public ImageSource WheelImg
        {
            get { return ImageSource.FromResource($"LibraryWindow.Images.WheelOfFortune.{WheelImgString}"); }
        }

        int[] uitkomsten1 = { 0, 50, 60, 95, 55, 50, 200, 50, 90, 50, 85, 50, 75, 80, 50, 75, 50, 300, 50, 65, 50, 70, 75, 400 };
        int[] uitkomsten2 = { 0, 50, 60, 95, 55, 50, 200, 50, 90, 50, 85, 50, 0, 80, 50, 75, 50, 300, 50, 65, 50, 70, 75, 400 };
        int[] uitkomsten3 = { 0, 50, 60, 95, 55, 50, 0, 50, 90, 50, 85, 50, 0, 80, 50, 75, 50, 300, 0, 65, 50, 70, 75, 400 };
        int[] uitkomsten4 = { 0, 50, 60, 0, 55, 50, 0, 50, 90, 0, 85, 50, 0, 80, 50, 0, 50, 300, 0, 65, 50, 0, 75, 400 };
        int[] uitkomsten5 = { 300, 0, 0, 300, 0, 0, 300, 0, 0, 300, 0, 0, 300, 0, 0, 300, 0, 0, 300, 0, 0, 300, 0, 0 };

        Random _rnd = new Random();
        Random _mySpeed = new Random();

        DateTime _lastStop = new DateTime();

        private DateTime _clock;
        public DateTime Clock
        {
            get { return _clock; }
            set { _clock = value; OnPropertyChanged(); }
        }

        private int _hourControle = 1;
        private int _secondControle = 0;

        private string _currentWin;
        public string CurrentWin
        {
            get { return _currentWin; }
            set { _currentWin = value; OnPropertyChanged(); }
        }

        private string _winGrid = "false";
        public string WinGrid
        {
            get { return _winGrid; }
            set { _winGrid = value; OnPropertyChanged(); }
        }

        bool draaiPressed = false;
        bool stopPressed = false;




        public WheelOfFortune()
        {
            InitializeComponent();
            BindingContext = this;
            Account();

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                //Clock = DateTime.Now;
                OnPropertyChanged("Tokens");
                try
                {
                    ApiWrapper.GetUserInfo();
                }
                catch
                {
                    DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                    Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
                return true; // True = Repeat again, False = Stop the timer
            });

            double height = 530;
            double width = 360;
            double uitkomst = Math.Tan(width / height);
        }

        private async void Btn_Draai_Pressed(object sender, EventArgs e)
        {
            Play();
        }

        private async void Play()
        {
            if (!draaiPressed)
            {
                draaiPressed = true;
                btnStop.IsEnabled = false;
                try
                {
                    if (Tokens >= 65)
                    {
                        WinAmount += (int)(WinAmount * 0.1);

                        blnShouldStay = true;
                        await ApiWrapper.UpdateTokens(-65, _sender);
                        await ApiWrapper.GetUserInfo();
                        OnPropertyChanged("Tokens");

                        ButtonSwitch = "False";
                        MyAngle = _rnd.Next(120, 145) * 15 + 7.5;
                        Cost += 65;
                        GameCount += 1;

                        await imgWheel.RotateTo(MyStartPoint, 0);
                        await imgWheel.RotateTo(MyAngle, (uint)_mySpeed.Next(6000, 14000), Easing.CubicOut);

                        btnStop.IsEnabled = true;
                        Storyboard_Completed();
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Te weinig tokens", "U moet minimaal 65 tokens in bezit hebben om te kunnen draaien!", "OK");
                        btnStop.IsEnabled = true;
                    }
                    draaiPressed = false;
                }
                catch
                {
                    await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                    await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
            }
        }

        private void OnSwiped0(object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    // Handle the swipe
                    break;
                case SwipeDirection.Right:
                    // Handle the swipe
                    break;
                case SwipeDirection.Up:
                    // Handle the swipe
                    Play();
                    break;
                case SwipeDirection.Down:
                    // Handle the swipe
                    break;
            }
        }
        private void OnSwiped(object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    // Handle the swipe
                    break;
                case SwipeDirection.Right:
                    // Handle the swipe
                    break;
                case SwipeDirection.Up:
                    // Handle the swipe
                    break;
                case SwipeDirection.Down:
                    // Handle the swipe
                    Play();
                    break;
            }
        }


        private async void Storyboard_Completed()
        {
            double a = (MyAngle - 7.5) % 360 / 15;
            int verschil = WinAmount;
            if (GameCount == 1)
            {
                if (uitkomsten1[(int)a] == 0)
                {
                    Bankrupt();
                }
                else
                {
                    WinAmount += uitkomsten1[(int)a];
                }
            }
            else if (GameCount == 2)
            {
                if (uitkomsten2[(int)a] == 0)
                {
                    Bankrupt();
                }
                else
                {
                    WinAmount += (int)Math.Floor(uitkomsten2[(int)a] * 1.0);
                }
            }
            else if (GameCount == 3)
            {
                if (uitkomsten3[(int)a] == 0)
                {
                    Bankrupt();
                }
                else
                {
                    WinAmount += (int)Math.Floor(uitkomsten3[(int)a] * 1.0);
                }
            }
            else if (GameCount == 4)
            {
                if (uitkomsten4[(int)a] == 0)
                {
                    Bankrupt();
                }
                else
                {
                    WinAmount += (int)Math.Floor(uitkomsten4[(int)a] * 1.0);
                }
            }
            else if (GameCount >= 5)
            {
                if (uitkomsten5[(int)a] == 0)
                {
                    Bankrupt();
                }
                else
                {
                    WinAmount += uitkomsten5[(int)a];
                    GameCount = 5;
                }
            }

            if (WinAmount - verschil <= 0)
            {
                CurrentWin = "Bankrupt!";
            }
            else
            {
                CurrentWin = $"+{WinAmount - verschil}";
            }

            MyStartPoint = (MyAngle / 360) + (MyAngle % 360) - (7.5 / 2);

            switch (GameCount)
            {
                case 0:
                    WheelImgString = "wheel.png";
                    break;
                case 1:
                    WheelImgString = "wheel2.png";
                    break;
                case 2:
                    WheelImgString = "wheel3.png";
                    break;
                case 3:
                    WheelImgString = "wheel4.png";
                    break;
                case 4:
                    MyAngle = 7.5;
                    MyStartPoint = 7.5;
                    await imgWheel.RotateTo(7.5, 0);
                    WheelImgString = "wheel5.png";
                    break;
                default:
                    // niks te vinden hier
                    break;
            }

            WinGrid = "true";
            await lbWin.TranslateTo(0, myGrid.Height / 2, 750, Easing.SinOut);
            await Task.Delay(500);
            await lbWin.FadeTo(0, 1000);
            lbWin.TranslationY = -150;
            lbWin.Opacity = 1;
            WinGrid = "false";
            ButtonSwitch = "True";
            blnShouldStay = false;
        }

        private async void Btn_Stop_Pressed(object sender, EventArgs e)
        {
            if (!stopPressed)
            {
                stopPressed = true;
                try
                {
                    await ApiWrapper.UpdateTokens(WinAmount, _sender);
                    await ApiWrapper.GetUserInfo();
                }
                catch
                {
                    await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                }

                //WinAmount += database.Tokens;
                Bankrupt();
                Cost = 0;
                stopPressed = false;
            }
        }

        private async void Bankrupt()
        {
            GameCount = 0;
            MyAngle = 7.5;
            MyStartPoint = 7.5;
            WheelImgString = "wheel.png";
            WinAmount = 0;
            await imgWheel.RotateTo(7.5, 0);

            btnStop.IsEnabled = false;

            Time_Difference();

            _lastStop = DateTime.Now; //to database
            //stopLabel.Text = _lastStop.ToString();
        }

        private void Time_Difference()
        {
            int _s = Clock.Subtract(_lastStop).Seconds;
            int _m = Clock.Subtract(_lastStop).Minutes;
            int _h = Clock.Subtract(_lastStop).Hours;
            int _d = Clock.Subtract(_lastStop).Days;
            //verschilLabel.Text = $"{_d} days {_h}:{_m}:{_s}";
            _hourControle = _h;
            _secondControle = _s;
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            if (gridRules.IsVisible == true)
            {
                gridRules.IsVisible = false;
            }
            else
            {
                gridRules.IsVisible = true;
            }

        }

        private async void BackButton_Pressed(object sender, EventArgs e)
        {
            try
            {
                if (blnShouldStay || WinAmount > 0)
                {
                    if (blnShouldStay == true)
                    {
                        await App.Current.MainPage.DisplayAlert("Let op!", "Wacht totdat het rad is uitgedraaid.", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Let op!", "Vergeet niet om op stop te drukken om je pot veilig te stellen op je account.", "OK");
                    }
                }
                else
                {
                    await Navigation.PushModalAsync(new NavigationPage(new MainPage()));
                }
            }
            catch
            {
                await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }
    }
}