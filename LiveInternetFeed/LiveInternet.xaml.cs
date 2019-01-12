using LiveInternetFeed.Model;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace LiveInternetFeed
{
	public partial class LiveInternet : Window
	{
		private const String appName = "LiveInternet";
		public static readonly ILog log = LogManager.GetLogger(typeof(LiveInternet));

		public LiveInternet()
		{
			InitializeComponent();
			XmlConfigurator.Configure(new FileInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "LiveInternetFeed.exe.config")));
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

			log.Info(String.Format("URL : {0}", MainModel.url));
			log.Info(String.Format("TIMEOUT : {0}", MainModel.time));
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