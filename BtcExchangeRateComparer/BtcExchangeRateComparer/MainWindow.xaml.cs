using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
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
            _krakenClient = new KrakenClient.KrakenClient();

            var checkingK = new TimeSpan(0, 0, 5);
            var checkingB = new TimeSpan(0, 0, 5);
            var vm = new ViewModel(0, 0, 5000.0m, 5000.0m, null, checkingK, checkingB);
            _viewModel = vm;

            _kTimer = new Timer(o =>
            {
                vm.CurrentKrakenRate = GetKrakenExchangeRate();
                CompareExchangeRates(vm);

            }, null, TimeSpan.Zero, checkingK);
            _bTimer = new Timer(o =>
            {
                vm.CurrentBitfinexRate = GetBitfinexExchangeRate();
                CompareExchangeRates(vm);

            }, null, TimeSpan.Zero, checkingB);

        }


        private static void CompareExchangeRates(ViewModel vm)
        {
            if (vm.CurrentBitfinexRate - vm.CurrentKrakenRate > vm.MaxDifferenceKrakenToBitfinex ||
                vm.CurrentKrakenRate - vm.CurrentBitfinexRate > vm.MaxDifferenceBitfinexToKraken)
            {
                Process.Start(vm.SelectedPath);
            }
        }

        private decimal GetKrakenExchangeRate()
        {
            var response = _krakenClient.GetTicker(new List<string>() { "XBTUSD" });
            var rate = (string)((JsonArray)((JsonObject)((JsonObject)response["result"])["XXBTZUSD"])["c"])[0];
            var rateDecimal = Convert.ToDecimal(rate);
            return rateDecimal;
        }

        private string GetHexString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(String.Format("{0:x2}", b));
            }
            return sb.ToString();
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

        public void selectFileButton_Click(object sender, RoutedEventArgs e)
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
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_OnGotFocus;
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text != string.Empty) return;

            tb.Text = "Dezimalzahl";
            tb.GotFocus += TextBox_OnGotFocus;
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.CheckingIntervallBitfinex = new TimeSpan(0, 0, 0, 0, (int)Convert.ToDecimal(bitfinexIntervall.Text) * 1000);
            _viewModel.CheckingIntervallKraken = new TimeSpan(0, 0, 0, 0, (int)Convert.ToDecimal(krakenIntervall.Text) * 1000);
            _viewModel.MaxDifferenceBitfinexToKraken = Convert.ToDecimal((bitfinexToKraken.Text));
            _viewModel.MaxDifferenceKrakenToBitfinex = Convert.ToDecimal(krakenToBitfinex.Text);
            _viewModel.SelectedPath = selectedFileTextBox.Text;
            _bTimer.Change(_viewModel.CheckingIntervallBitfinex, _viewModel.CheckingIntervallBitfinex);
            _kTimer.Change(_viewModel.CheckingIntervallKraken, _viewModel.CheckingIntervallKraken);
        }
    }
}
