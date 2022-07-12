using Xamarin.Forms;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using Xamarin.Essentials;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
//using App
//using Google.Apis.Drive.v3;
//using Google.Apis.Drive.v3.Data;
//using Google.Apis.Requests;
//using System.Net;

using Plugin.Messaging;
using System;
using Xamarin.Forms.Xaml;

namespace easyCRM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Table_Page : ContentPage
    {
        // For Google Sheets
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "easyCRM";
        static readonly string SpreadSheetId = "1ruYVW8LOGUY5NqivMTNCQSK_7Jkm9LyMyVZ84tzrjy0";
        static readonly string sheet = "List_1";
        static readonly string range = $"{sheet}!A:U";

        // add ScrollView
        ScrollView scrollView;
        static SheetsService service;

        Image refreshTableBtnImage = new Image { Source = "refreshBtn.jpg", HeightRequest = 80, WidthRequest = 80 };

        // Grid settings
        Grid grid = new Grid
        {
            RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }, // Height = GridLength.Auto
                },
            ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
        };

        // Dictionary for delete buttons in table
        Dictionary<int, Button> dictForDelBtns = new Dictionary<int, Button>();

        public Table_Page()
        {
            // Connect to JSON login file
            WorkWithFile().Wait();

            ColumnNames();
            tableMake();
            //lbl.GestureRecognizers.Add(tapGestureRecognizer);

            // Add grid to scrollView (important when rotate the screen ) - user can scroll
            scrollView = new ScrollView { Content = grid };
            Content = scrollView;
        }

        private void ColumnNames()
        {

            //https://stackoverflow.com/questions/37331756/google-sheets-api-v4-how-to-get-the-last-row-with-value
            // Table Data
            var rowsValues = service.Spreadsheets.Values.Get(SpreadSheetId, range).Execute().Values;

            // Set values for grid
            grid.Children.Add(new Label
            {
                Text = rowsValues[0][1].ToString(),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 18,
                BackgroundColor = Xamarin.Forms.Color.Black,
                TextColor = Xamarin.Forms.Color.White
            }, 0, 0); // column, row
            grid.Children.Add(new Label
            {
                Text = rowsValues[0][2].ToString(),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 18,
                BackgroundColor = Xamarin.Forms.Color.Black,
                TextColor = Xamarin.Forms.Color.White
            }, 1, 0); // column, row
            grid.Children.Add(new Label
            {
                Text = rowsValues[0][3].ToString(),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 18,
                BackgroundColor = Xamarin.Forms.Color.Black,
                TextColor = Xamarin.Forms.Color.White
            }, 2, 0); // column, row
            grid.Children.Add(new Label
            {
                Text = rowsValues[0][17].ToString(),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 18,
                BackgroundColor = Xamarin.Forms.Color.Black,
                TextColor = Xamarin.Forms.Color.White
            }, 3, 0); // column, row

            // add btn to grid
            grid.Children.Add(refreshTableBtnImage, 4, 0); // column, row

            var tapGestureRecognizer_forRefreshBtn = new TapGestureRecognizer();
            tapGestureRecognizer_forRefreshBtn.Tapped += (s, e) => {

                // Brand new Carousel_Page
                CarouselPage Carousel_Page = new CarouselPage();
                Carousel_Page.Children.Add(new Table_Page());
                Carousel_Page.Children.Add(new MainPage());
                Carousel_Page.Children.Add(new GSheetBrowser_Page());

                Application.Current.MainPage = Carousel_Page;

                //grid.Children.Add(new Label { Text = grid.Children[10].ToString() },0,0);

                //while (grid.Children[5] != null)
                //{
                //    grid.Children.RemoveAt(5);
                //}


                //grid.Children.Clear();
                //grid.ColumnDefinitions.Clear();
                //grid.RowDefinitions.Clear();
                //dictForDelBtns.Clear();


                //ColumnNames();
                //tableMake();
            };
            refreshTableBtnImage.GestureRecognizers.Add(tapGestureRecognizer_forRefreshBtn);

            // Grid padding
            grid.Padding = 7;
        }
        private void tableMake()
        {

            //https://stackoverflow.com/questions/37331756/google-sheets-api-v4-how-to-get-the-last-row-with-value
            // Table Data
            var rowsValues = service.Spreadsheets.Values.Get(SpreadSheetId, range).Execute().Values;

            // count amout of rows in table
            //rowsValues.Count

            // command for tapGestureRecognizer
            //Command tappedCommand = new Command(() =>
            //{
            //    TestMessage(); - deleted
            //});

            //// Taps
            //var tapGestureRecognizer = new TapGestureRecognizer { Command = tappedCommand };
            //tapGestureRecognizer.NumberOfTapsRequired = 2; // double tap

            if (rowsValues != null && rowsValues.Count > 0) //
            {
                // https://developers.google.com/sheets/api/reference/rest/v4/spreadsheets.values/batchClearByDataFilter
                for (int i = 1; i < rowsValues.Count; i++)
                {
                    var column = rowsValues[i];
                    //if (column.GetEnumerator() == null /* || column[2] == null || column[3] == null || column[17] == null*/) //&& column.ToString() == "0"
                    //{
                    //    i += 1;
                    //}
                    try
                    {
                        //// Just flag for correct exception catch work
                        //Boolean exxceptionCatchFlag = column[1].ToString() == null;

                        // Add new row to grid
                        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                        // Button name
                        int valueForBtnDict = i + 1;
                        string btnName = valueForBtnDict.ToString();

                        // Add new Button to Dictionary
                        dictForDelBtns.Add(i, new Button { Text = "Delete", FontSize = 12, Padding = -10, ClassId = btnName, /*BindingContext = new ViewModel(btnName)*/  });

                        // Add data to grid from Google Sheets
                        grid.Children.Add(new Label { Text = column[1].ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 0, i); // column, row
                        grid.Children.Add(new Label { Text = column[2].ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 1, i); // column, row
                        grid.Children.Add(new Label { Text = column[3].ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 2, i); // column, row
                        grid.Children.Add(new Label { Text = column[17].ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 3, i); // column, row
                        grid.Children.Add(dictForDelBtns[i], 4, i); // column, row 

                        // Button Click
                        dictForDelBtns[i].Clicked += /*async*/ (sender, args) => {

                            // Delete from Google Sheets table
                            service.Spreadsheets.Values.Clear(new ClearValuesRequest(), SpreadSheetId, $"A{Int32.Parse(btnName)}:U{Int32.Parse(btnName)}").Execute();

                            // Hide row
                            grid.RowDefinitions[Int32.Parse(btnName)-1].Height = new GridLength(0);

                            // Delete from dict
                            dictForDelBtns.Remove(i);
                            //grid.RowDefinitions.GetEnumerator();
                            //for (int j = 1; j <= 5; j++)
                            //{
                            //    grid.Children.RemoveAt(((Int32.Parse(btnName) - 1)) * 5);
                            //};
                            //grid.RowDefinitions.RemoveAt(Int32.Parse(btnName) - 2);
                        }; // Delete row from table // grid.Children.Clear() - for clear table on phone
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // Hide row
                        //grid.RowDefinitions[i].Height = new GridLength(0);
                    }
                }
            }
        }

        // Connect to Google Sheets table
        public async Task WorkWithFile()
        {
            GoogleCredential credential;

            //https://stackoverflow.com/questions/61228071/how-to-add-json-file-to-xamarin-forms
            //https://stackoverflow.com/questions/8399994/working-with-system-threading-tasks-taskstream-instead-of-stream
            using (var stream = await FileSystem.OpenAppPackageFileAsync("easycrm_login_file.json"))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        //public class ViewModel
        //{
        //    public ViewModel(string btnName)
        //    {
        //        int i = Int32.Parse(btnName);
        //        //service.Spreadsheets.Values.Clear(new ClearValuesRequest(), SpreadSheetId, $"A{i}:U{i}").Execute();
        //        //grid.Children.Add();
        //    }

        //    //public Command MyViewModelCommand { get; set; } = new Command(() =>
        //    //{
        //    //    Console.WriteLine("You have invoked a command in the view model!");
        //    //});
        //}
    }
}