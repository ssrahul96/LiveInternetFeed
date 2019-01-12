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
        internal static String url = String.Empty;
        internal static int time = 0;
        internal static String toastExe = Path.Combine(Environment.CurrentDirectory, "Toast.exe");
        internal const String toastMessage = "Unable to connect SAM";
    }
}
