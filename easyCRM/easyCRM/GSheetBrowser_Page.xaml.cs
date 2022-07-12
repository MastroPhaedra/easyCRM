using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace easyCRM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GSheetBrowser_Page : ContentPage
    {

        private string URL = "https://docs.google.com/spreadsheets/d/1ruYVW8LOGUY5NqivMTNCQSK_7Jkm9LyMyVZ84tzrjy0/edit#gid=0";
        public GSheetBrowser_Page()
        {
            var browser = new WebView
            {
                Source = URL,
            };
            Content = browser;
        }
    }
}