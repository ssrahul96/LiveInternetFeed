using InteractiveDataDisplay.WPF;
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
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using LiveInternetFeed.Model;
using System.Configuration;
using System.Text.RegularExpressions;

namespace LiveInternetFeed
{
    public partial class MainWindow : Window
    {
        private const String appName = "LiveInternet";
        public MainWindow()
        {
            InitializeComponent();
            Uri uriResult;
            if (ValidHttpURL(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["samurl"]), out uriResult))
                MainModel.url = uriResult?.AbsoluteUri;
            else
            {
                MessageBox.Show("URL is not Proper", appName);
                Environment.Exit(0);
                return;

            }
            try
            {
                MainModel.time = 1000 * Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["time"]);
            }
            catch
            {
                MessageBox.Show("Time is not Proper", appName);
                Environment.Exit(0);
                return;
            }

        }

        public bool ValidHttpURL(string s, out Uri resultURI)
        {
            if (!Regex.IsMatch(s, @"^https?:\/\/", RegexOptions.IgnoreCase))
                s = "http://" + s;

            if (Uri.TryCreate(s, UriKind.Absolute, out resultURI))
                return (resultURI.Scheme == Uri.UriSchemeHttp ||
                        resultURI.Scheme == Uri.UriSchemeHttps);

            return false;
        }
    }

    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
