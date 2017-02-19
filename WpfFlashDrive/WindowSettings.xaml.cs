using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfFlashDrive
{
    /// <summary>
    /// Interaction logic for WindowSettings.xaml
    /// </summary>
    public partial class WindowSettings : Window
    {
        System.Windows.Controls.RadioButton[] radioButtons = null;


        public WindowSettings()
        {
            InitializeComponent();
            InitializeIconsWindowSettings();

            Left = System.Windows.SystemParameters.WorkArea.Right - Width;
            Top = System.Windows.SystemParameters.WorkArea.Bottom - Height - 100;

            radioButtons = new[]
            {
                RadioButtonScanAutomatically,
                RadioButtonScanManuallyRequest,
                RadioButtonScanManually
            };
        }

        private void InitializeIconsWindowSettings()
        {
            Icon = new BitmapImage(new Uri(ConfigurationManager.AppSettings["Icon_Main"], UriKind.Relative));

            WindowSettingsImage.Source =
                new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgSettings"], UriKind.Relative));
        }

        private void ButtonSetDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "Select folder for watching";
                dialog.SelectedPath = Properties.Settings.Default.DirectoryForWatcing;
                DialogResult result = dialog.ShowDialog();
                if (!string.IsNullOrWhiteSpace(dialog.SelectedPath) && !string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    Properties.Settings.Default.DirectoryForWatcing = dialog.SelectedPath;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    throw new Exception("Please, choose correct directory!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ButtonDisableDrive_OnClick(object sender, RoutedEventArgs e)
        {
            DriveInfo driveInfo = new DriveInfo("ssss");
        }

        private void ButtonEnableDrive_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (radioButtons.Where(item => item.IsChecked == true).Select(item => item.Name).Any() !=false)
            {
                Properties.Settings.Default.TypeOfScan = radioButtons.Where(item => item.IsChecked == true).Select(item => item.Name).First().ToString(); 
                Properties.Settings.Default.Save();
            }

            System.Windows.MessageBox.Show(Properties.Settings.Default.DirectoryForWatcing);
            System.Windows.MessageBox.Show(Properties.Settings.Default.TypeOfScan);
        }
    }
}