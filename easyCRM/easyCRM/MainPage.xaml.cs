using Xamarin.Forms;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using Xamarin.Essentials;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public partial class MainPage : ContentPage
    {
        public TableView tabelview;
        //ImageCell ic;
        //TableSection fotosection;
        ViewCell vcButtons, vcPickers, vcDatePicker;
        Button sms_btn, call_btn, send_btn;
        StackLayout stackButtons, stackPickers, stackDatePicker;
        string phoneNumber;

        //https://docs.microsoft.com/en-us/dotnet/api/xamarin.forms.entrycell?view=xamarin-forms
        EntryCell NameCell = new EntryCell
        {
            Label = "Nimi: ",
            Placeholder = "Sisesta klienti nimi",
            Keyboard = Keyboard.Default,
        };

        EntryCell TelefoneCell = new EntryCell
        {
            Label = "Telefon: ",
            Placeholder = "Sisesta tel.number",
            Keyboard = Keyboard.Telephone,
            Text = ""
        };

        EntryCell BuyUSBCell = new EntryCell
        {
            Label = "Ostab USB: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell BuyDVDCell = new EntryCell
        {
            Label = "Ostab DVD: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell ClientDataCarrierCell = new EntryCell
        {
            Label = "Andmekandja omanikult: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell VHSAmountCell = new EntryCell
        {
            Label = "VHS kogus: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell VHS_CAmountCell = new EntryCell
        {
            Label = "VHS-C kogus: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell Hi8AmountCell = new EntryCell
        {
            Label = "Hi8 kogus: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell MiniDVAmountCell = new EntryCell
        {
            Label = "MiniDV kogus: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell AudioAmountCell = new EntryCell
        {
            Label = "Audio kogus: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell TapeAmountCell = new EntryCell
        {
            Label = "Lintide kogus: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell TapeLengthCell = new EntryCell
        {
            Label = "Linti pikkus: ",
            Placeholder = "Sisesta min",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        EntryCell VideoEditAmountCell = new EntryCell
        {
            Label = "Montaaž: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        Picker PlaceOfIssuePicker = new Picker
        {
            Title="Välja võtmise aeg",
        };

        EntryCell PriceCell = new EntryCell
        {
            Label = "Kokku: ",
            Placeholder = "Sisesta kogus",
            Keyboard = Keyboard.Numeric,
            Text = "0"
        };

        Picker DonePicker = new Picker
        {
            Title = "Valmis",
        };

        //https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/datepicker
        DatePicker WithdrawalDatePicker = new DatePicker{};

        //IMAGE

        EntryCell CommentsCell = new EntryCell
        {
            Label = "Kommentaarid: ",
            Placeholder = "Sisesta kommentaarid",
            Keyboard = Keyboard.Text,
            Text = "Tühi"
        };

        //
        //
        // For Google Sheets
        // https://www.youtube.com/watch?v=afTiNU6EoA8&t=174s

        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "easyCRM";
        static readonly string SpreadSheetId = "1ruYVW8LOGUY5NqivMTNCQSK_7Jkm9LyMyVZ84tzrjy0";
        static readonly string sheet = "List_1";
        static SheetsService service;

        public MainPage()
        {
            // Buttons
            call_btn = new Button { Text = "Helista" };
            sms_btn = new Button { Text = "Saada sms" };
            send_btn = new Button { Text = "Ok" };

            // StackLayout for buttons
            stackButtons = new StackLayout
            {
                Children = { call_btn, sms_btn, send_btn },
                Orientation = StackOrientation.Horizontal
            };

            // vc for buttons
            vcButtons = new ViewCell();
            vcButtons.View = stackButtons;

            call_btn.Clicked += PhoneCallClick;
            sms_btn.Clicked += MessengSendClick;
            send_btn.Clicked += SendDataClick;

            // PlaceOfIssuePicker
            // https://stackoverflow.com/questions/56239550/xamarin-forms-default-value-for-picker
            PlaceOfIssuePicker.Items.Add("Kontor");
            PlaceOfIssuePicker.Items.Add("Pakiautomaat");
            PlaceOfIssuePicker.Items.Add("Kuller");
            PlaceOfIssuePicker.SelectedIndex = 0;

            // DonePicker
            DonePicker.Items.Add("TRUE");
            DonePicker.Items.Add("FALSE");
            DonePicker.SelectedIndex = 1;

            // Label for PlaceOfIssuePicker
            Label lbl_poiPicker = new Label
            {
                Text = "Koht:",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            // Label for DonePicker
            Label lbl_donePicker = new Label
            {
                Text = "Valmis:",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            // StackLayout for PlaceOfIssuePicker & DonePicker
            stackPickers = new StackLayout
            {
                Children = { lbl_poiPicker, PlaceOfIssuePicker, lbl_donePicker, DonePicker, /*WithdrawalDatePicker*/ },
                Orientation = StackOrientation.Horizontal
            };

            // vc for pickers
            vcPickers = new ViewCell();
            vcPickers.View = stackPickers;

            // Label for WithdrawalDatePicker
            Label lbl_wdPicker = new Label 
            {
                Text = "Väljavõtmise aeg:",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            // StackLayout for WithdrawalDatePicker
            stackDatePicker = new StackLayout
            {
                Children = { lbl_wdPicker, WithdrawalDatePicker },
                Orientation = StackOrientation.Horizontal
            };

            // vc for WithdrawalDatePicker
            vcDatePicker = new ViewCell();
            vcDatePicker.View = stackDatePicker;

            // Table
            tabelview = new TableView
            {
                Intent = TableIntent.Form, //Могут быть ещё Menu, Data, Settings
                Root = new TableRoot("Andmete sisestamine")
                {
                    new TableSection("Klienti andmed ")
                    {
                        NameCell,
                        TelefoneCell,
                    },
                    new TableSection("Tellimuse andmed ")
                    {
                        BuyUSBCell,
                        BuyDVDCell,
                        ClientDataCarrierCell,
                        VHSAmountCell,
                        VHS_CAmountCell,
                        Hi8AmountCell,
                        MiniDVAmountCell,
                        AudioAmountCell,
                        TapeAmountCell,
                        TapeLengthCell,
                        VideoEditAmountCell,
                        //PlaceOfIssuePicker,
                        //PriceCell, // FOR EDITING
                        //DoneBoolean,
                        //WithdrawalDatePicker,
                        CommentsCell,
                    },
                    new TableSection()
                    {
                        vcPickers,
                        vcDatePicker
                    },
                    new TableSection("Nupud ")
                    {
                        vcButtons,
                    },
                }
            };
            Content = tabelview;
            WorkWithFile().Wait();
            //ReadEntry();
            //CreateEntry();
            //UpdateEntry();
            //DeleteEntry();
        }

        public async Task WorkWithFile()
        {
            GoogleCredential credential;

            //https://stackoverflow.com/questions/61228071/how-to-add-json-file-to-xamarin-forms
            //https://stackoverflow.com/questions/8399994/working-with-system-threading-tasks-taskstream-instead-of-stream
            using (var stream = await FileSystem.OpenAppPackageFileAsync("easycrm_login_file.json"))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
                //using (var reader = new StreamReader(stream))
                //{
                //    var fileContents = await reader.ReadToEndAsync();
                //}
            }

            //using (var stream = new FileStream("easycrm_login_file.json", FileMode.Open, FileAccess.Read))
            //{
            //    credential = GoogleCredential.FromStream(stream)
            //        .CreateScoped(Scopes);
            //}


            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

        }

        //static void ReadEntry()
        //{
        //    string tableData = "";
        //    var range = $"{sheet}!A1:F10";
        //    var request = service.Spreadsheets.Values.Get(SpreadSheetId, range);

        //    var response = request.Execute();
        //    var values = response.Values;

        //    Label lbl = new Label
        //    {
        //        Text = "",
        //    };

        //    if (values != null && values.Count > 0)
        //    {
        //        foreach (var row in values)
        //        {
        //            tableData += row[5] + " " + row[4] + " " + row[3] + " " + row[1];
        //            lbl.Text = tableData;
        //        }
        //    }
        //    else
        //    {
        //        lbl.Text = "No data found";
        //    }
        //}

        async void SendDataClick(object sender, EventArgs e)
        {
            Guid id = Guid.NewGuid();
            var range = $"{sheet}!A:U";
            var valueRange = new ValueRange();

            //19 columns + 1 ID column
            //NameCell,
            //TelefoneCell,
            //BuyUSBCell,
            //BuyDVDCell,
            //ClientDataCarrierCell,
            //VHSAmountCell,
            //VHS_CAmountCell,
            //Hi8AmountCell,
            //MiniDVAmountCell,
            //AudioAmountCell, 
            //TapeAmountCell,
            //TapeLengthCell,
            //VideoEditAmountCell,
            ////PlaceOfIssuePicker,
            //PriceCell,
            ////DoneBoolean,
            ////WithdrawalDatePicker,
            //CommentsCell,
            var objectList = new List<object>() { id, NameCell.Text, TelefoneCell.Text, DateTime.Today.ToString("dd.MM.yy"), 
                BuyUSBCell.Text, BuyDVDCell.Text, ClientDataCarrierCell.Text, VHSAmountCell.Text, VHS_CAmountCell.Text, 
                Hi8AmountCell.Text, MiniDVAmountCell.Text, AudioAmountCell.Text, TapeAmountCell.Text, TapeLengthCell.Text, 
                VideoEditAmountCell.Text, PlaceOfIssuePicker.SelectedItem.ToString(), 
                PriceCell.Text, DonePicker.SelectedItem.ToString(), WithdrawalDatePicker.Date.ToString("dd.MM.yy"), "Pilt", CommentsCell.Text };
            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();

            clearFields();
        }

        //static void UpdateEntry()
        //{
        //    var range = $"{sheet}!D10";
        //    var valueRange = new ValueRange();

        //    var objectList = new List<object>() { "updated" };
        //    valueRange.Values = new List<IList<object>> { objectList };

        //    var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadSheetId, range);
        //    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        //    var updateResponce = updateRequest.Execute();
        //}

        //static void DeleteEntry()
        //{
        //    var range = $"{sheet}!A11:F";
        //    var requestBody = new ClearValuesRequest();

        //    var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadSheetId, range);
        //    var deleteResponce = deleteRequest.Execute();
        //}

        private void PhoneCallClick(object sender, EventArgs e)
        {
            sendData();
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
            {
                phoneDialer.MakePhoneCall(phoneNumber);
            };
        }

        private void MessengSendClick(object sender, EventArgs e)
        {
            //
            sendData();
            var smsMessenger = CrossMessaging.Current.SmsMessenger;
            if (smsMessenger.CanSendSms)
            {
                smsMessenger.SendSms(phoneNumber, "Tere! \n");
            };
        }

        public void sendData()
        {
            //if (TelefoneCell.Text.Int32() < 7)
            //{

            //}
            phoneNumber = "+372" + TelefoneCell.Text.ToString();
            //emailSend = EmailCell.Text.ToString();
        }

        public void clearFields() 
        {
            NameCell.Text = "";
            TelefoneCell.Text = "";
            BuyUSBCell.Text = "0";
            BuyDVDCell.Text = "0";
            ClientDataCarrierCell.Text = "0";
            VHSAmountCell.Text = "0";
            VHS_CAmountCell.Text = "0";
            Hi8AmountCell.Text = "0";
            MiniDVAmountCell.Text = "0";
            AudioAmountCell.Text = "0";
            TapeAmountCell.Text = "0";
            TapeLengthCell.Text = "0";
            VideoEditAmountCell.Text = "0";
            PlaceOfIssuePicker.SelectedIndex = 0;
            PriceCell.Text = "0";
            DonePicker.SelectedIndex = 1;
            //WithdrawalDatePicker = new DatePicker();
            CommentsCell.Text = "Tühi";
        }

        //https://developers.google.com/drive/api/v3/reference?apix=true
        //https://www.youtube.com/watch?v=KPDGtLeNClQ

        //https://github.com/stevenchang0529/XamarinGoogleDriveRest/blob/master/XamarinGoogleDriveRest/XamarinGoogleDriveRest/DriveServiceHelper.cs
        //https://stackoverflow.com/questions/65345646/using-google-drive-api-with-xamarinforms

    }
}