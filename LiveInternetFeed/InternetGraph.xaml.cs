using LiveCharts;
using LiveCharts.Configurations;
using LiveInternetFeed.Model;
using log4net;
using log4net.Config;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LiveInternetFeed
{
	public partial class InternetGraph : UserControl, INotifyPropertyChanged
	{
		public static readonly ILog log = LogManager.GetLogger(typeof(InternetGraph));
		private double _axisMax;
		private double _axisMin;
		private double _trend;

		private string _url;
		private int _timeout;

		private const String appName = "LiveInternet";
		private Uri uriResult;

		private string website = String.Empty;

		public InternetGraph()
		{
			InitializeComponent();

			var mapper = Mappers.Xy<MeasureModel>()
			   .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
			   .Y(model => model.Value);           //use the value property as Y

			//lets save the mapper globally.
			Charting.For<MeasureModel>(mapper);

			//the values property will store our values array
			ChartValues = new ChartValues<MeasureModel>();

			//lets set how to display the X Labels
			DateTimeFormatter = value => new DateTime((long)value).ToString("hh:mm:ss");

			//AxisStep forces the distance between each separator in the X axis
			AxisStep = TimeSpan.FromSeconds(10).Ticks;
			//AxisUnit forces lets the axis know that we are plotting seconds
			//this is not always necessary, but it can prevent wrong labeling
			AxisUnit = TimeSpan.TicksPerSecond;

			SetAxisLimits(DateTime.Now);

			//The next code simulates data changes every 300 ms

			IsReading = false;

			DataContext = this;

			XmlConfigurator.Configure(new FileInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "LiveInternetFeed.exe.config")));

			_url = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["checkurl"]);

			int.TryParse(System.Configuration.ConfigurationManager.AppSettings["timeout"], out _timeout);

			if (Environment.GetCommandLineArgs().Length == 2 && Environment.GetCommandLineArgs()[1].Equals("startup"))
				InjectStopOnClick(new object(), new RoutedEventArgs());

		}

		public ChartValues<MeasureModel> ChartValues { get; set; }
		public Func<double, string> DateTimeFormatter { get; set; }
		public double AxisStep { get; set; }
		public double AxisUnit { get; set; }

		public double AxisMax
		{
			get { return _axisMax; }
			set
			{
				_axisMax = value;
				OnPropertyChanged("AxisMax");
			}
		}

		public double AxisMin
		{
			get { return _axisMin; }
			set
			{
				_axisMin = value;
				OnPropertyChanged("AxisMin");
			}
		}

		public string btnStartStop
		{
			get
			{
				return IsReading ? "Stop" : "Start";
			}
		}

		public string URL
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
			}
		}

		public bool EnableTextBox { get { return !IsReading; } }

		public bool IsReading { get; set; }

		private void Read()
		{
			var r = new Random();

			while (IsReading)
			{
				Thread.Sleep(_timeout * 1000);
				var now = DateTime.Now;

				try
				{
					using (WebClient wc = new MyWebClient())
					{
						wc.DownloadString(_url);

						_trend = (DateTime.Now - now).TotalMilliseconds;
					}
				}
				catch (Exception ex)
				{
					_trend = 0;
					log.Error(ex.Message);
					Process.Start(String.Format("\"{0}\"", MainModel.toastExe), String.Format("\"{0} : {1}\" \"{2}{3}{4}\"", MainModel.toastMessage, website, DateTime.Now, Environment.NewLine, ex.Message));
				}

				ChartValues.Add(new MeasureModel
				{
					DateTime = now,
					Value = _trend
				});

				SetAxisLimits(now);

				if (ChartValues.Count > 100) ChartValues.RemoveAt(0);
			}
		}

		private void SetAxisLimits(DateTime now)
		{
			AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
			AxisMin = now.Ticks - TimeSpan.FromSeconds(100).Ticks; // and 8 seconds behind
		}

		private void InjectStopOnClick(object sender, RoutedEventArgs e)
		{
			if (!IsReading)
			{
				if (ValidHttpURL(_url, out uriResult))
				{
					_url = uriResult?.AbsoluteUri;
					website = uriResult?.Host;
					ApplySettings("checkurl", _url);
					ApplySettings("timeout", _timeout.ToString());
				}
				else
				{
					MessageBox.Show("URL is not Proper", appName);
					Environment.Exit(0);
					return;
				}

				//log.Info(String.Format("URL : {0}", _url));
				//log.Info(String.Format("TIMEOUT : {0}", _timeout));
			}

			IsReading = !IsReading;
			if (IsReading) Task.Factory.StartNew(Read);
			OnPropertyChanged("btnStartStop");
			OnPropertyChanged("EnableTextBox");
			OnPropertyChanged("URL");
		}

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged implementation

		private class MyWebClient : WebClient
		{
			protected override WebRequest GetWebRequest(Uri uri)
			{
				WebRequest w = base.GetWebRequest(uri);
				w.Timeout = 1000;
				return w;
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
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

		public void ApplySettings(string key, string value)
		{
			var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var settings = configFile.AppSettings.Settings;
			if (settings[key] == null)
			{
				settings.Add(key, value);
			}
			else
			{
				settings[key].Value = value;
			}
			configFile.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
		}

		private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			var textBox = sender as TextBox;
			e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
		}
	}
}