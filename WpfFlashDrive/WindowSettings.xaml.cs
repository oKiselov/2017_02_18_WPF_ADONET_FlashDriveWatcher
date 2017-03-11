using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

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
            try
            {
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

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method initializes icons for window's icon and main image with direct images 
        /// </summary>
        private void InitializeIconsWindowSettings()
        {
            try
            {
                Icon = new BitmapImage(new Uri(ConfigurationManager.AppSettings["Icon_Main"], UriKind.Relative));

                WindowSettingsImage.Source =
                    new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgSettings"], UriKind.Relative));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
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
            try
            {
                string tempTypeOfScan =
                    radioButtons.Where(button => button.IsChecked == true)
                        .Select(button => button.Name)
                        .FirstOrDefault();
                if (!string.IsNullOrEmpty(tempTypeOfScan))
                {
                    Properties.Settings.Default.TypeOfScan = tempTypeOfScan;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}