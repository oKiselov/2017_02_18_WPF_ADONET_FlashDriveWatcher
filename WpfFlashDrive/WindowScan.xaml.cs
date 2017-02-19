using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace WpfFlashDrive
{
    /// <summary>
    /// Interaction logic for WindowScan.xaml
    /// </summary>
    public partial class WindowScan : Window
    {
        public WindowScan()
        {
            InitializeComponent();
            InitializeIconsWindowScan();

            Left = System.Windows.SystemParameters.WorkArea.Right - Width;
            Top = System.Windows.SystemParameters.WorkArea.Bottom - Height-100;
        }

        private void InitializeIconsWindowScan()
        {
            Icon = new BitmapImage(new Uri(ConfigurationManager.AppSettings["Icon_Main"], UriKind.Relative));

            WindowScanImage.Source = new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgScan"], UriKind.Relative));
        }
    }
}
