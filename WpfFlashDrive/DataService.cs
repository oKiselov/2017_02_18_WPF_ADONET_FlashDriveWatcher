using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Windows;

namespace WpfFlashDrive
{
    /// <summary>
    /// Following classes represent information about titles of each columns in tables in BookStoreDB
    /// </summary>
    public static class DataProvider
    {
        public static string SqlServer
        {
            get { return string.Format("System.Data.SqlClient"); }
        }

        public static string OleDb
        {
            get { return string.Format("System.Data.OleDb"); }
        }

        public static string Odbc
        {
            get { return string.Format("System.Data.Odbc"); }
        }
    }

    /// <summary>
    /// Class consist of names of tables 
    /// </summary>
    public static class Tables
    {
        public static string CopiedFiles
        {
            get { return string.Format("CopiedFiles"); }
        }

        public static string DailyActivity
        {
            get { return string.Format("DailyActivity"); }
        }

        public static string DeletedDuplicates
        {
            get { return string.Format("DeletedDuplicates"); }
        }
    }

    /// <summary>
    /// Class describes entity of CopiedFiles Table 
    /// </summary>
    public static class TableCopiedFiles
    {
        public static string FullName
        {
            get { return string.Format("FullName"); }
        }

        public static string Size
        {
            get { return string.Format("Size"); }
        }

        public static string DateOfCreation
        {
            get { return string.Format("DateOfCreation"); }
        }
    }

    /// <summary>
    /// Class describes entity of DailyActivity Table 
    /// </summary>
    public static class TableDailyActivity
    {
        public static string Id
        {
            get { return string.Format("Id"); }
        }

        public static string Date
        {
            get { return string.Format("Date"); }
        }

        public static string Operation
        {
            get { return string.Format("Operation"); }
        }

        public static string FullName
        {
            get { return string.Format("FullName"); }
        }

        public static string Size
        {
            get { return string.Format("Size"); }
        }
    }

    /// <summary>
    /// Class describes entity of DeletedDuplicates Table 
    /// </summary>
    public static class TableDeletedDuplicates
    {
        public static string FullName
        {
            get { return string.Format("FullName"); }
        }

        public static string Size
        {
            get { return string.Format("Size"); }
        }

        public static string DateOfDeleting
        {
            get { return string.Format("DateOfDeleting"); }
        }
    }

    /// <summary>
    /// Class describes entity of Operations: Copy or Delete  
    /// </summary>
    public static class Operation
    {
        public static string Copy
        {
            get { return string.Format("Copy"); }
        }

        public static string Delete
        {
            get { return string.Format("Delete"); }
        }
    }

    /// <summary>
    /// Class - main data operator. DB - FilesFlashDrive
    /// </summary>
    public class DataService
    {
        private IDbConnection _connection = null;

        private string _strConnectionString;

        /// <summary>
        /// Method returns current type of data connection 
        /// </summary>
        /// <param name="strProvider">class for Providers</param>
        /// <returns>connection element</returns>
        private IDbConnection GetConnection(string strProvider)
        {
            IDbConnection connectionDb = null;
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(strProvider);
                connectionDb = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return connectionDb;
        }

        /// <summary>
        /// Method returns your current connection string from App.config file 
        /// </summary>
        /// <param name="strProvider">Your data provider</param>
        /// <returns></returns>
        public void SetConnectionString(string strProvider)
        {
            try
            {
                _strConnectionString = ConfigurationManager.ConnectionStrings[strProvider].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method opens connection to current DB
        /// </summary>
        /// <param name="strProvider">class for Providers</param>
        public void OpenConnection(string strProvider)
        {
            try
            {
                _connection = GetConnection(strProvider);
                _connection.ConnectionString = _strConnectionString;
                _connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method closes current connection
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                _connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method inserts info into DeletedDuplicates Table 
        /// </summary>
        /// <param name="FullName">File name</param>
        /// <param name="lSize">file size in bytes</param>
        /// <param name="date">date of file deleting</param>
        public void InsertIntoDuplicates(string FullName, long lSize, DateTime date)
        {
            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO " +
                                                    Tables.DeletedDuplicates +
                                                    " (" + TableDeletedDuplicates.FullName +
                                                    " , " + TableDeletedDuplicates.Size +
                                                    " , " + TableDeletedDuplicates.DateOfDeleting +
                                                    " ) " +
                                                    "VALUES ('" + FullName +
                                                    "' , " + lSize +
                                                    ", '" + date.ToString("yyyy-MM-dd HH:mm:ss") +
                                                    "')");
                    cmd.ExecuteNonQuery();
                }
                InsertIntoDailyActivity(FullName, lSize, DateTime.UtcNow, Operation.Delete);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method inserts info into CopiedFiles Table 
        /// </summary>
        /// <param name="FullName">File name</param>
        /// <param name="lSize">file size in bytes</param>
        /// <param name="date">date of file creation</param>
        public void InsertIntoCopied(string FullName, long lSize, DateTime date)
        {
            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO " +
                                                    Tables.CopiedFiles +
                                                    " (" + TableCopiedFiles.FullName +
                                                    " , " + TableCopiedFiles.Size +
                                                    " , " + TableCopiedFiles.DateOfCreation +
                                                    " ) " +
                                                    "VALUES ( '" + FullName +
                                                    "' , " + lSize +
                                                    ", '" + date.ToString("yyyy-MM-dd HH:mm:ss") +
                                                    "')");
                    cmd.ExecuteNonQuery();
                }
                InsertIntoDailyActivity(FullName, lSize, DateTime.UtcNow, Operation.Copy);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method inserts info into DailyActivity Table with statistics 
        /// </summary>
        /// <param name="FullName">File name</param>
        /// <param name="lSize">file size in bytes</param>
        /// <param name="date">date of file operation</param>
        /// <param name="strOperation">kind of operation</param>
        public void InsertIntoDailyActivity(string FullName, long lSize, DateTime date, string strOperation)
        {
            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO " +
                                                    Tables.DailyActivity +
                                                    " (" + TableDailyActivity.Date +
                                                    " , " + TableDailyActivity.Operation +
                                                    " , " + TableDailyActivity.FullName +
                                                    " , " + TableDailyActivity.Size +
                                                    " ) " +
                                                    "VALUES('" + date.ToString("yyyy.MM.dd HH:mm:ss") +
                                                    "', '" + strOperation +
                                                    "', '" + FullName +
                                                    "' , " + lSize +
                                                    " )");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Method creates file with statistics - Statistics.txt in the root 
        /// </summary>
        public void GetStatistics()
        {
            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT * FROM " + Tables.DailyActivity + " ");
                    var dataReader = cmd.ExecuteReader();

                    using (StreamWriter file = new StreamWriter("Statistics.txt", false))
                    {
                        while (dataReader.Read())
                        {
                            file.WriteLine(dataReader[TableDailyActivity.Id] + "\t" +
                                           dataReader[TableDailyActivity.Date] + "\t" +
                                           dataReader[TableDailyActivity.Operation] + "\t" +
                                           dataReader[TableDailyActivity.Size] + "\t" +
                                           dataReader[TableDailyActivity.FullName]);
                        }
                    }
                }
                MessageBox.Show("File with statistics was completed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}