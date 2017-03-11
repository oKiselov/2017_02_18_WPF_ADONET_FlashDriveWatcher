using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Configuration;
using System.IO;

namespace WpfFlashDrive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.ComponentModel.IContainer components;

        // for windows messages hooking 
        private List<DriveInfo> _listOfDriveInfo = null;

        UsbDeviceInfo usbDevice;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                SetContextMenu();

                SetNotifyIcon();

                InitializeIconsMainWindow();

                Left = System.Windows.SystemParameters.WorkArea.Right - Width;
                Top = System.Windows.SystemParameters.WorkArea.Bottom - Height;
                Visibility = Visibility.Hidden;
                WindowStyle = WindowStyle.None;

                // fix information about drives at the moment 
                _listOfDriveInfo =
                    new List<DriveInfo>(DriveInfo.GetDrives()).Where(drive => drive.DriveType == DriveType.Removable)
                        .ToList();

                Properties.Settings.Default.FlashDrive = string.Empty;

                usbDevice = new UsbDeviceInfo("id", "name");
                Task.Run(() =>
                {
                    usbDevice.DetectUsbDrive(new List<UsbDeviceInfo>());
                });

                usbDevice.UsbAdd += CheckAddedUsb;
                usbDevice.UsbRemoved += CheckRemovedUsb;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        // method for adding drive to list 
        public void CheckAddedUsb(object sender, UsbEventArgs eventArgs)
        {
            try
            {
                DriveInfo driveInfo = null;
                driveInfo = FileOperator.GetDriveInfoAddUsb(_listOfDriveInfo);
                if (driveInfo != null)
                {
                    DialogResult dialogResult =
                        System.Windows.Forms.MessageBox.Show(
                            @"USB device was added. Name:" + driveInfo.Name +
                            @". To start scanning your device press button Start Scan!", @"USB", MessageBoxButtons.OK);
                    Properties.Settings.Default.FlashDrive = driveInfo.Name;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Event for removing drive from list 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void CheckRemovedUsb(object sender, UsbEventArgs eventArgs)
        {
            Properties.Settings.Default.FlashDrive = string.Empty;
            _listOfDriveInfo = FileOperator.SetDriveInfoRemoveUsb(_listOfDriveInfo); 
        }

        /// <summary>
        /// Method sets all images and icons 
        /// </summary>
        private void InitializeIconsMainWindow()
        {
            try
            {
                Icon = new BitmapImage(new Uri(ConfigurationManager.AppSettings["Icon_Main"], UriKind.Relative));

                var brushBeginToScan = new System.Windows.Controls.Image();
                brushBeginToScan.Source =
                    new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgScan"], UriKind.Relative));
                ButtonBeginToScan.Content = brushBeginToScan;

                var brushSettings = new System.Windows.Controls.Image();
                brushSettings.Source =
                    new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgSettings"], UriKind.Relative));
                ButtonSettings.Content = brushSettings;

                var brushStat = new System.Windows.Controls.Image();
                brushStat.Source =
                    new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgStatistics"], UriKind.Relative));
                ButtonStatistics.Content = brushStat;

                var brushHide = new System.Windows.Controls.Image();
                brushHide.Source =
                    new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgHide"], UriKind.Relative));
                ButtonHide.Content = brushHide;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Notification of NotifyIcon 
        /// </summary>
        private void SetNotifyIcon()
        {
            try
            {
                // create notify icon 
                components = new Container();
                notifyIcon = new NotifyIcon(components);

                // The Icon property sets the icon that will appear
                // in the systray for this application.
                notifyIcon.Icon = new Icon(ConfigurationManager.AppSettings["Icon_Main"]);

                // The ContextMenu property sets the menu that will
                // appear when the systray icon is right clicked.
                notifyIcon.ContextMenu = contextMenu;

                // The Text property sets the text that will be displayed,
                // in a tooltip, when the mouse hovers over the systray icon.
                notifyIcon.Text = string.Format("WpfFlashDrive");
                notifyIcon.Visible = true;

                notifyIcon.DoubleClick += Click_OpenMainWindow;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method sets new contextMenu 
        /// </summary>
        private void SetContextMenu()
        {
            try
            {
                contextMenu = new System.Windows.Forms.ContextMenu();

                // Initialize contextMenu 
                contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]{
                new System.Windows.Forms.MenuItem() { Index = 0, Text = @"Open Window" },
                new System.Windows.Forms.MenuItem() { Index = 1, Text = @"Start Scanning" },
                new System.Windows.Forms.MenuItem() { Index = 2, Text = @"Settings" },
                new System.Windows.Forms.MenuItem() { Index = 3, Text = @"Close" },
            });

                // Initialize menuItem 
                contextMenu.MenuItems[0].Click += Click_OpenMainWindow;
                contextMenu.MenuItems[1].Click += Click_StartScan;
                contextMenu.MenuItems[2].Click += Click_Settings;
                contextMenu.MenuItems[3].Click += Click_CloseMainWindow;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Open main window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Click_OpenMainWindow(object sender, EventArgs eventArgs)
        {
            try
            {
                // Show the form when the user double clicks on the notify icon.
                // Set the WindowState to normal if the form is minimized.

                if (Visibility == Visibility.Hidden)
                {
                    Visibility = Visibility.Visible;
                }
                WindowStyle = WindowStyle.None;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Open settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Click_Settings(object sender, EventArgs eventArgs)
        {
            try
            {
                WindowSettings window = new WindowSettings();
                window.InitializeComponent();
                window.Visibility = Visibility.Visible;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method starts scan process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Click_StartScan(object sender, EventArgs eventArgs)
        {
            try
            {
                WindowScan window = new WindowScan();
                window.InitializeComponent();
                window.Visibility = Visibility.Visible;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method closes main window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Click_CloseMainWindow(object sender, EventArgs eventArgs)
        {
            try
            {
                notifyIcon.Dispose();
                Properties.Settings.Default.FlashDrive = string.Empty;
                Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method hides main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonHide_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method creates file with statistics - Statistics.txt in the root 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStatistics_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataService dataService = new DataService();
                dataService.SetConnectionString(WpfFlashDrive.DataProvider.SqlServer);
                dataService.OpenConnection(WpfFlashDrive.DataProvider.SqlServer);
                dataService.GetStatistics();
                dataService.CloseConnection();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}
