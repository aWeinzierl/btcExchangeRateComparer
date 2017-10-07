using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace courseAlarm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var func = new TriggeredFunction(delegate { });

            var btcTrigger = new BoundaryTrigger<decimal>(func, decimal.MinValue, decimal.MaxValue);
            var xbtTrigger = new BoundaryTrigger<decimal>(func, decimal.MinValue, decimal.MaxValue);
            
            var vm = new ViewModel(btcTrigger,xbtTrigger, null);

            //var crawler = new Crawler(
        }

        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            var result = fileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                selectedFileTextBox.Text = fileDialog.FileName;
            }


        }
    }
}
