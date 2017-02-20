using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;

namespace WpfFlashDrive
{
    /// <summary>
    /// Class for information about USB drives  
    /// </summary>
    public class UsbDeviceInfo
    {
        // type of delegate 
        public delegate void UsbDelegate(object sender, UsbEventArgs eventArgs);

        // event for new drive 
        public event UsbDelegate UsbAdd;

        // event for removed drive 
        public event UsbDelegate UsbRemoved; 

        public string strDeviceID { get; private set; }

        public string strName { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceID"></param>
        public UsbDeviceInfo(string deviceID, string name)
        {
            this.strDeviceID = deviceID;
            strName = name; 
        }

        /// <summary>
        /// Method support searching process - for changes in devices environment
        /// </summary>
        /// <param name="listPrevDevices"></param>
        public void DetectUsbDrive(List<UsbDeviceInfo> listPrevDevices)
        {
            while (true)
            {
                List<UsbDeviceInfo> devices = new List<UsbDeviceInfo>();

                ManagementObjectCollection collection;
                using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                {
                    collection = searcher.Get();
                }

                foreach (var device in collection)
                {
                    device.SystemProperties.SyncRoot.ToString();
                    devices.Add(new UsbDeviceInfo(
                        (string)device.GetPropertyValue("DeviceID"), 
                        (string)device.GetPropertyValue("Name")));
                }

                if (listPrevDevices == null || !listPrevDevices.Any()) // So we don't detect already plugged in devices the first time.
                    listPrevDevices = devices;

                var insertionDevices = devices.Where(d => !listPrevDevices.Any(d2 => d2.strDeviceID == d.strDeviceID));
                if (insertionDevices != null && insertionDevices.Any())
                {
                    // Event that amount of drives had changed 
                    UsbAdd(this, new UsbEventArgs());
                }

                var removedDevices = listPrevDevices.Where(d => !devices.Any(d2 => d2.strDeviceID == d.strDeviceID));
                if (removedDevices != null && removedDevices.Any())
                {
                    // Event that amount of drives had changed 
                    UsbRemoved(this, new UsbEventArgs());
                }

                listPrevDevices = devices;
                collection.Dispose();
            }
        }
    }
}