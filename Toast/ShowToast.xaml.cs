using PeanutButter.Toast;
using System;
using System.Collections.Generic;
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

namespace Toast
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String appName = "Toast";
        private static Mutex mutex = null;
        public MainWindow()
        {
            InitializeComponent();


            bool createdNew;

            Mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                Environment.Exit(0);
            }

            String[] cla = Environment.GetCommandLineArgs();

            if (cla.Length != 3)
                Environment.Exit(0);

            this.Hide();

            Toaster toaster = new Toaster();
            toaster.Show(cla[1], new Exception(cla[2]));
        }

        public static Mutex Mutex { get => mutex; set => mutex = value; }
    }
}
