using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BitfinexApi;
using Jayrock.Json;
using Newtonsoft.Json;

namespace btcExchangeRateComparer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModel _viewModel;
        private readonly KrakenClient.KrakenClient _krakenClient;
        private Timer _bTimer;
        private Timer _kTimer;


        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _krakenClient = new KrakenClient.KrakenClient();

            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(e.ToString(), "Error");
                });
            }
            var checkingK = new TimeSpan(0, 0, 5);
            var checkingB = new TimeSpan(0, 0, 6);
            var vm = new ViewModel(0, 0, 0, 0, null, checkingK, checkingB, false);
            _viewModel = vm;

            _kTimer = new Timer(o =>
            {
                try
                {
                    vm.CurrentKrakenRate = GetKrakenExchangeRate();
                    CompareExchangeRates(vm);
                }
                catch (Exception) { }


                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        krakenKurs.Content = vm.CurrentKrakenRate.ToString(CultureInfo.InstalledUICulture);
                        differenceTextBox.Content = Math.Abs(vm.CurrentKrakenRate - vm.CurrentBitfinexRate).
                            ToString(CultureInfo.InstalledUICulture); ;
                    });

            }, null, TimeSpan.Zero, checkingK);
            _bTimer = new Timer(o =>
            {
                try
                {
                    vm.CurrentBitfinexRate = GetBitfinexExchangeRate();
                }
                catch (Exception) { }

                CompareExchangeRates(vm);

                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        bitfinexKurs.Content = vm.CurrentBitfinexRate.ToString(CultureInfo.InstalledUICulture);
                        differenceTextBox.Content = Math.Abs(vm.CurrentKrakenRate - vm.CurrentBitfinexRate).
                            ToString(CultureInfo.InstalledUICulture);
                    });

            }, null, TimeSpan.Zero, checkingB);


            activateAlarmButton.IsEnabled = true;
            bitfinexIntervall.Text = "6";
            krakenIntervall.Text = "5";
            krakenToBitfinex.Text =
                _viewModel.MaxDifferenceKrakenToBitfinex.ToString(CultureInfo.InstalledUICulture);
            bitfinexToKraken.Text =
                _viewModel.MaxDifferenceBitfinexToKraken.ToString(CultureInfo.InstalledUICulture);
        }


        private void CompareExchangeRates(ViewModel vm)
        {
            if ((vm.CurrentBitfinexRate - vm.CurrentKrakenRate > vm.MaxDifferenceKrakenToBitfinex ||
                   vm.CurrentKrakenRate - vm.CurrentBitfinexRate > vm.MaxDifferenceBitfinexToKraken)
                  &&
                  vm.AlarmActive)
            {
                vm.AlarmActive = false;

                try
                {
                    Process.Start(vm.SelectedPath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    activateAlarmButton.IsEnabled = true;
                });
            }
        }

        private decimal GetKrakenExchangeRate()
        {
            var response = _krakenClient.GetTicker(new List<string>() { "XBTUSD" });
            var rate = ((JsonArray)((JsonObject)((JsonObject)response["result"])["XXBTZUSD"])["c"])[0];
            var rateDecimal = Convert.ToDecimal(rate, new CultureInfo("en-US"));
            return rateDecimal;

        }

        private decimal GetBitfinexExchangeRate()
        {
            const string url = "https://api.bitfinex.com/v2/ticker/tBTCUSD";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject<List<decimal>>(reader.ReadToEnd())[6];
            }
        }



        public void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            var result = fileDialog.ShowDialog();

            if (result != true) return;

            selectedFileTextBox.Text = fileDialog.FileName;

        }

        public void TextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text == "Dezimalzahl")
            {
                tb.Text = string.Empty;
            }
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text != string.Empty) return;

            tb.Text = "Dezimalzahl";
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.CheckingIntervallBitfinex = new TimeSpan(0, 0, 0, 0, (int)(decimal.Parse(bitfinexIntervall.Text, CultureInfo.InstalledUICulture) * 1000));
            _viewModel.CheckingIntervallKraken = new TimeSpan(0, 0, 0, 0, (int)(decimal.Parse(krakenIntervall.Text, CultureInfo.InstalledUICulture) * 1000));
            _viewModel.MaxDifferenceBitfinexToKraken = decimal.Parse((bitfinexToKraken.Text), CultureInfo.InstalledUICulture);
            _viewModel.MaxDifferenceKrakenToBitfinex = decimal.Parse(krakenToBitfinex.Text, CultureInfo.InstalledUICulture);
            _viewModel.SelectedPath = selectedFileTextBox.Text;
            _bTimer.Change(_viewModel.CheckingIntervallBitfinex, _viewModel.CheckingIntervallBitfinex);
            _kTimer.Change(_viewModel.CheckingIntervallKraken, _viewModel.CheckingIntervallKraken);
        }

        private void ActivateAlarmButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.AlarmActive = true;
            activateAlarmButton.IsEnabled = false;
        }
    }
}
