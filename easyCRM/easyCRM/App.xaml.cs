using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace easyCRM
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            CarouselPage Carousel_Page = new CarouselPage();
            Carousel_Page.Children.Add(new Table_Page());
            Carousel_Page.Children.Add(new MainPage());
            Carousel_Page.Children.Add(new GSheetBrowser_Page());
            //Carousel_Page.Children.Add(new MainPage());

            MainPage = Carousel_Page;
        }

        protected override void OnStart()
        {
            //test commit
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
