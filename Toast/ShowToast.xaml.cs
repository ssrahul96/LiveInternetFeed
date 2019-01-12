using PeanutButter.Toast;
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

namespace Toast
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            String[] cla = Environment.GetCommandLineArgs();

            if (cla.Length != 3)
                Environment.Exit(0);

            this.Hide();

            Toaster toaster = new Toaster();
            toaster.Show(cla[1], new Exception(cla[2]));
        }
    }
}
