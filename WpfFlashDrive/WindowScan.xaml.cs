using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfFlashDrive
{
    /// <summary>
    /// Interaction logic for WindowScan.xaml
    /// </summary>
    public partial class WindowScan : Window
    {
        // Data provider 
        private DataService _dataService = null;

        // token for cancellation process 
        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

        public WindowScan()
        {
            InitializeComponent();
            InitializeIconsWindowScan();

            // Main WindowScan settings 
            Left = System.Windows.SystemParameters.WorkArea.Right - Width;
            Top = System.Windows.SystemParameters.WorkArea.Bottom - Height - 100;

        }

        /// <summary>
        /// Method initializes icons for window's icon and main ButtonStartScanning with direct images 
        /// </summary>
        private void InitializeIconsWindowScan()
        {
            try
            {
                Icon = new BitmapImage(new Uri(ConfigurationManager.AppSettings["Icon_Main"], UriKind.Relative));

                var brushBeginToScan = new System.Windows.Controls.Image();
                brushBeginToScan.Source =
                    new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImgScan"], UriKind.Relative));
                ButtonStartScanning.Content = brushBeginToScan;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method starts scanning and synchronization process 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStartScanning_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                StartScanAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _cancelToken.Cancel();

                LabelScan.Content = string.Format("Canceled");
                ProgressBarScan.IsIndeterminate = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void StartScanAsync()
        {

            // Blocks ButtonStartScanning for second click 
            ButtonStartScanning.IsEnabled = false;
            try
            {
                string strPathToFlashDrive = string.Empty;

                // Inspection for empty pathes to main directory or flash drive 
                if (string.IsNullOrEmpty(Properties.Settings.Default.DirectoryForWatching)
                    || string.IsNullOrEmpty(Properties.Settings.Default.TypeOfScan)
                    || string.IsNullOrEmpty(Properties.Settings.Default.FlashDrive))
                {
                    throw new Exception("Please, set correct directory for watching or type of scanning!");
                }

                // Run progressbar
                ProgressBarScan.IsIndeterminate = true;

                // list of files on HDD 
                List<FileInfo> listOfFilesOnHDD = new List<FileInfo>();
                LabelScan.Content = string.Format("Browse HDD...");
                listOfFilesOnHDD.AddRange(await WalkDirectoryTree(Properties.Settings.Default.DirectoryForWatching));


                // list of files on flash drive 
                List<FileInfo> listOfFilesOnFlashDrive = new List<FileInfo>();
                LabelScan.Content = string.Format("Browse flash drive...");
                listOfFilesOnFlashDrive.AddRange(await WalkDirectoryTree(Properties.Settings.Default.FlashDrive));

                List<FileInfo> listOfDublicates = new List<FileInfo>();
                List<FileInfo> listOfUnique = new List<FileInfo>();

                LabelScan.Content = string.Format("Browse for duplicates...");
                
                // Search coincedences in two lists  
                foreach (var fileOnDrive in listOfFilesOnFlashDrive)
                {
                    bool bIsDuplicateOnHDD =
                        listOfFilesOnHDD.Any(
                            item => item.Name == fileOnDrive.Name && item.Length == fileOnDrive.Length);

                    if (bIsDuplicateOnHDD == true)
                    {
                        listOfDublicates.Add(fileOnDrive);
                    }
                    else
                    {
                        listOfUnique.Add(fileOnDrive);
                    }
                }

                LabelScan.Content = string.Format("Deleting duplicates...");

                // delete all duplicates 
                await DeleteDuplicatesAsync(listOfDublicates);

                // fix information about deleted duplicates in DB
                await InsertIntoDuplicates(listOfDublicates);

                LabelScan.Content = string.Format("Copying uniques...");

                // copy all unique files 
                await CopyAsync(listOfUnique);

                // fix information about copied files into DB 
                await InsertIntoCopied(listOfUnique);
                //MessageBox.Show("Completed Button" + DateTime.UtcNow);
                LabelScan.Content = string.Format("Complete");

                ProgressBarScan.IsIndeterminate = false;
                ProgressBarScan.Maximum = 100;
                ProgressBarScan.Value = 100;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Disable block ButtonStartScanning for second click 
            ButtonStartScanning.IsEnabled = true;
        }

        /// <summary>
        /// Method runs traverse from main directory to subdirectories 
        /// </summary>
        /// <param name="strPathToDirectory">Path to root directory</param>
        /// <returns></returns>
        private Task<List<FileInfo>> WalkDirectoryTree(string strPathToDirectory)
        {
            return Task.Run(() =>
            {
                List<FileInfo> retList = new List<FileInfo>();
                try
                {
                    FileOperator.WalkDirectoryTree(new DirectoryInfo(strPathToDirectory), retList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return retList;
            });
        }

        /// <summary>
        /// Method copies all unique files into HDD 
        /// </summary>
        /// <param name="listOfUnique">list of info for unique files </param>
        /// <returns></returns>
        private Task CopyAsync(List<FileInfo> listOfUnique)
        {
            return Task.Factory.StartNew(() =>
            {
                ParallelOptions parOpts = new ParallelOptions();
                parOpts.CancellationToken = _cancelToken.Token;
                parOpts.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

                try
                {
                    Parallel.For(0, listOfUnique.Count, index =>
                    {
                        parOpts.CancellationToken.ThrowIfCancellationRequested();

                        File.Copy(listOfUnique[index].FullName,
                            string.Format(Properties.Settings.Default.DirectoryForWatching + "\\" +
                                          listOfUnique[index].Name), true);
                    });

                }
                catch (Exception exTask)
                {
                    MessageBox.Show(exTask.Message);
                }
            });
        }


        /// <summary>
        /// Method deletes all duplicates from flash-drive 
        /// </summary>
        /// <param name="listOfDublicates">list of info for duplicates </param>
        /// <returns></returns>
        private Task DeleteDuplicatesAsync(List<FileInfo> listOfDublicates)
        {
            return Task.Factory.StartNew(() =>
            {
                ParallelOptions parOpts = new ParallelOptions();
                parOpts.CancellationToken = _cancelToken.Token;
                parOpts.MaxDegreeOfParallelism = System.Environment.ProcessorCount;

                try
                {
                    Parallel.For(0, listOfDublicates.Count, index =>
                    {
                        parOpts.CancellationToken.ThrowIfCancellationRequested();

                        File.Delete(listOfDublicates[index].FullName);
                    });

                }
                catch (Exception exTask)
                {
                    MessageBox.Show(exTask.Message);
                }
            });
        }


        /// <summary>
        /// Method inserts info into DeletedDuplicates Table 
        /// </summary>
        /// <param name="listOfDublicates">list of info for duplicates </param>
        /// <returns></returns>
        private Task InsertIntoDuplicates(List<FileInfo> listOfDublicates)
        {
            return Task.Run(() =>
            {
                try
                {
                    ParallelOptions parOpts = new ParallelOptions();
                    parOpts.CancellationToken = _cancelToken.Token;

                    _dataService = new DataService();
                    _dataService.SetConnectionString(WpfFlashDrive.DataProvider.SqlServer);
                    _dataService.OpenConnection(WpfFlashDrive.DataProvider.SqlServer);

                    foreach (var duplicate in listOfDublicates)
                    {
                        _dataService.InsertIntoDuplicates(
                            duplicate.FullName,
                            duplicate.Length,
                            DateTime.UtcNow);
                    }

                    _dataService.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        /// <summary>
        /// Method inserts info into CopiedFiles Table 
        /// </summary>
        /// <param name="listOfUnique">list of info for unique files</param>
        /// <returns></returns>
        private Task InsertIntoCopied(List<FileInfo> listOfUnique)
        {
            return Task.Run(() =>
            {
                try
                {
                    ParallelOptions parOpts = new ParallelOptions();
                    parOpts.CancellationToken = _cancelToken.Token;

                    _dataService = new DataService();
                    _dataService.SetConnectionString(WpfFlashDrive.DataProvider.SqlServer);
                    _dataService.OpenConnection(WpfFlashDrive.DataProvider.SqlServer);

                    foreach (var unique in listOfUnique)
                    {
                        _dataService.InsertIntoCopied(
                            unique.FullName,
                            unique.Length,
                            unique.CreationTime);
                    }

                    _dataService.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}