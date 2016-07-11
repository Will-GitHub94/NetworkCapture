﻿using System;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace IPCapture
{
    /// <summary>
    /// Stores all the information required of the device the app is running on.
    /// </summary>
    /// <remarks>
    /// This class is instantiated when the app is first run (before the GUI is initialized)
    /// </remarks>
    public class Machine : INotifyPropertyChanged
    {
        private const string EMPTY = "-";

        private string _IPv4 = EMPTY;
        private string _IPv6 = EMPTY;
        private string _MACAddress = EMPTY;
        private string _SubnetMask = EMPTY;

        public string MachineName { get; set; }
        public string OperatingSystem { get; set; }
        public string OSArchitecture { get; set; }
        public string OSManufacturer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private object _lock = new object();

        public Machine()
        {
            this.IPv4 = getIPv4();
            this.IPv6 = getIPv6();
            this.MACAddress = getMACAddress();
            this.SubnetMask = getSubnetMask();

            this.MachineName = getMachineName();
            this.OperatingSystem = getOperatingSystem();
            this.OSArchitecture = getOSArchitecture();
            this.OSManufacturer = getOSManufacturer();
        }

        private string getIPv4()
        {
            try
            {
                IPHostEntry IPhostEntry = Dns.GetHostEntry(Dns.GetHostName());
                string IPv4 = EMPTY;
                foreach (IPAddress ip in IPhostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPv4 = ip.ToString();
                    }
                }
                return IPv4;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getIPv6()
        {
            try
            {
                string IPv6 = EMPTY;
                IPHostEntry IPhostEntry = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in IPhostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        IPv6 = ip.ToString();
                    }
                }
                return IPv6;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getMACAddress()
        {
            try
            {
                string MACAddress = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

                foreach (ManagementObject mo in mc.Get())
                {
                    MACAddress = (string)mo["MACAddress"];
                    break;
                }
                return MACAddress;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getSubnetMask()
        {
            try
            {
                string SubnetMask = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

                foreach (ManagementObject mo in mc.Get())
                {
                    string[] subnets = (string[])mo["IPSubnet"];
                    SubnetMask = subnets[0];
                }
                return SubnetMask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getOSArchitecture()
        {
            try
            {
                string OSArchitecture = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    OSArchitecture = (string)mo["OSArchitecture"];
                }
                return OSArchitecture;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getOperatingSystem()
        {
            try
            {
                string OperatingSystem = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    OperatingSystem = (string)mo["Caption"];
                }
                return OperatingSystem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getOSManufacturer()
        {
            try
            {
                string Manufacturer = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    Manufacturer = (string)mo["Manufacturer"];
                }
                return Manufacturer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SubnetMask
        {
            get { return this._SubnetMask; }
            set { setter(value, "SubnetMask", ref this._SubnetMask); }
        }

        public string MACAddress
        {
            get { return this._MACAddress; }
            set { setter(value, "MACAddress", ref this._MACAddress); }
        }

        public string IPv6
        {
            get { return this._IPv6; }
            set { setter(value, "IPv6", ref this._IPv6); }
        }

        public string IPv4
        {
            get { return this._IPv4; }
            set { setter(value, "IPv4", ref this._IPv4); }
        }

        private void setter(string val, string propertyName, ref string propertyVal)
        {
            lock (_lock)
            {
                if (val != propertyVal)
                {
                    propertyVal = val;
                    NotifyPropertyChanged(propertyName);
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
