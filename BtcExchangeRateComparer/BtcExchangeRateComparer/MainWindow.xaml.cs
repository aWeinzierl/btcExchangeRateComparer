using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jayrock.Json;

namespace courseAlarm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var checkingK = new TimeSpan(0, 0, 5);
            var checkingB = new TimeSpan(0, 0, 5);
            var vm = new ViewModel(0, 0, 5000.0m, 5000.0m, null, checkingK, checkingB);
            _viewModel = vm;

            var krakenTimer = new Timer(o =>
            {
                vm.CurrentKrakenRate = getKrakenExchangeRate();
                CompareExchangeRates(vm);

            }, null, TimeSpan.Zero, checkingK);
            var bitfinexTimer = new Timer(o =>
            {
                vm.CurrentBitfinexRate = getBitfinexExchangeRate();
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

        private decimal getKrakenExchangeRate()
        {
            var kc = new KrakenClient.KrakenClient();
            var response = kc.GetTicker(new List<string>(){ "XBTUSD" });
            var rate = (string)((JsonArray)((JsonObject)((JsonObject)response["result"])["XXBTZUSD"])["c"])[0];
            var rateDecimal = Convert.ToDecimal(rate);
            return rateDecimal;
        }
        private decimal getBitfinexExchangeRate()
        {
            return 0.0m;
        }

        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            var result = fileDialog.ShowDialog();

            if (result != true) return;

            selectedFileTextBox.Text = fileDialog.FileName;
            _viewModel.SelectedPath = fileDialog.FileName;
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
    }
}
