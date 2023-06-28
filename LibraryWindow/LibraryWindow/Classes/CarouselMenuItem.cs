using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LibraryWindow.Classes
{
    public class CarouselMenuItem
    { 
        private ImageSource _image;

        public ImageSource Source
        {
            get { return _image; }
        }

        private ContentPage _contentPage;

        public ContentPage MyContentPage
        {
            get { return _contentPage; }
        }

        public CarouselMenuItem(ImageSource image, ContentPage page)
        {
            _image = image;
            _contentPage = page;
        }
    }
}
