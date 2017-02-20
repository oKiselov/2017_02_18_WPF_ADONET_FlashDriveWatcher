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
        // array of radiobuttons
        System.Windows.Controls.RadioButton[] radioButtons = null;
        
        public WindowSettings()
        {
            InitializeComponent();
            InitializeIconsWindowSettings();

            // configuration of settings widow 
            Left = System.Windows.SystemParameters.WorkArea.Right - Width;
            Top = System.Windows.SystemParameters.WorkArea.Bottom - Height - 100;

            // radiobutton's array initialization 
            radioButtons = new[]
            {
                RadioButtonScanAutomatically,
                RadioButtonScanManuallyRequest,
            };

            if (Properties.Settings.Default.TypeOfScan == RadioButtonScanManuallyRequest.Name)
            {
                RadioButtonScanManuallyRequest.IsChecked = true;
            }
            else
            {
                RadioButtonScanAutomatically.IsChecked = true; 
            }

            TextBlockSettings.Text = Properties.Settings.Default.DirectoryForWatching;
        }

        /// <summary>
        /// Method initializes icons for window's icon and main image with direct images 
        /// </summary>
        private void InitializeIconsWindowSettings()
        {
            Icon = new BitmapImage(new Uri(ConfigurationManager.AppSettings["Icon_Main"], UriKind.Relative));

            WindowSettingsImage.Source =
                new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgSettings"], UriKind.Relative));
        }

        /// <summary>
        /// Method desribes settings and choosing the directory 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSetDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "Select folder for watching";
                dialog.SelectedPath = Properties.Settings.Default.DirectoryForWatching;
                DialogResult result = dialog.ShowDialog();
                if (!string.IsNullOrWhiteSpace(dialog.SelectedPath) && !string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    Properties.Settings.Default.DirectoryForWatching = dialog.SelectedPath;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    throw new Exception("Please, choose correct directory!");
                }
                TextBlockSettings.Text = Properties.Settings.Default.DirectoryForWatching;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string tempTypeOfScan =
                radioButtons.Where(button => button.IsChecked == true).Select(button => button.Name).FirstOrDefault();
            if (!string.IsNullOrEmpty(tempTypeOfScan))
            {
                Properties.Settings.Default.TypeOfScan = tempTypeOfScan;
                Properties.Settings.Default.Save();
            }
               
        }
    }
}