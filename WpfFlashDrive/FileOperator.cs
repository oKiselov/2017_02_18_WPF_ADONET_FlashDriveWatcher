using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace WpfFlashDrive
{
    public static class FileOperator
    {

        /// <summary>
        /// Method for traverse of all directories and subdirectories in 
        /// DirectoryForWatching 
        /// </summary>
        /// <param name="rootDirectory">DirectoryInfo type, contains information about directory and synchroniztion </param>
        /// <returns>List of FileInfo elements - information about all files inside current directory</returns>
        public static void WalkDirectoryTree(DirectoryInfo rootDirectory, List<FileInfo> listOfFiles)
        {
            List<FileInfo> listOFileInfoTemp = new List<FileInfo>();
            List<DirectoryInfo> templistOfDirectories = new List<DirectoryInfo>();

            try
            {
                listOFileInfoTemp = rootDirectory.GetFiles("*.*").ToList();
                if (listOFileInfoTemp.Count != 0)
                {
                    listOfFiles.AddRange(listOFileInfoTemp);
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                templistOfDirectories = rootDirectory.GetDirectories().ToList();
                if (templistOfDirectories.Count != 0)
                {
                    foreach (var directory in templistOfDirectories)
                    {
                        WalkDirectoryTree(directory, listOfFiles);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Method gets information about new drives and returns one new driveinfo 
        /// </summary>
        /// <param name="_listOfDriveInfo"></param>
        /// <returns></returns>
        public static DriveInfo GetDriveInfoAddUsb(List<DriveInfo> _listOfDriveInfo)
        {
            DriveInfo driveInfo = null;
            List<DriveInfo> listDriveTemp = new List<DriveInfo>(DriveInfo.GetDrives())
                .Where(drive => drive.DriveType == DriveType.Removable).ToList();

            // check that amount of removable discs has been changed 
            if (listDriveTemp.Count > _listOfDriveInfo.Count)
            {
                // search what disc is new 
                for (int i = 0; i < listDriveTemp.Count; i++)
                {
                    if (!_listOfDriveInfo.Any(drive => drive.Name == listDriveTemp[i].Name))
                    {
                        _listOfDriveInfo.Add(listDriveTemp[i]);
                        driveInfo = listDriveTemp[i];
                    }
                }
            }
            return driveInfo;
        }

        /// <summary>
        /// Method cuts of removed drive 
        /// </summary>
        /// <param name="_listOfDriveInfo"></param>
        public static List<DriveInfo> SetDriveInfoRemoveUsb(List<DriveInfo> _listOfDriveInfo)
        {
            return new List<DriveInfo>(DriveInfo.GetDrives())
                            .Where(drive => drive.DriveType == DriveType.Removable).ToList();
        }
    }
}