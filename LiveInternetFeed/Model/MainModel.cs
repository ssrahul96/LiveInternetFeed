using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveInternetFeed.Model
{
	class MainModel
	{
		internal static String toastExe = Path.Combine(Environment.CurrentDirectory, "Toast.exe");
		internal const String toastMessage = "Unable to connect";
	}

	public class MeasureModel
	{
		public DateTime DateTime { get; set; }
		public double Value { get; set; }
	}
}
