using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;
using System.Net.Sockets;
using System.Net;
using Overseer.DTOs.MonitoringAgent;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class SystemInformationMonitor : IMonitorable<SystemInformation>
    {
        private Logger _Logger;
        
        private SystemInformation _SystemInfo;

        public SystemInformationMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _SystemInfo = new SystemInformation();  // reset values

            _Logger.Log("System Information Snapshot:");

            GetConnectivityInfo();
            GetOSInfo();
            GetResourcesInfo();
            GetMiscellaneousInfo();

            _Logger.Log("Snapshot successful for: System Information");
        }

        public void DataCheck()
        {
            _Logger.Log("---------- SYSTEM INFO ----------");
            _Logger.Log("Machine Name: " + _SystemInfo.MachineName);
            _Logger.Log("IP Address: " + _SystemInfo.IPAddress);
            _Logger.Log("OS Name: " + _SystemInfo.OSName);
            _Logger.Log("Friendly OS Name: " + _SystemInfo.OSNameFriendly);
            _Logger.Log("OS Bitness: " + _SystemInfo.OSBitness);
            _Logger.Log("Processor Count: " + _SystemInfo.ProcessorCount);
            _Logger.Log("Total RAM: " + _SystemInfo.TotalMem.ToString("0.0"));
            _Logger.Log("Current Up Time (mins): " + _SystemInfo.UpTime.ToString(@"dd\.hh\:mm\:ss"));
            _Logger.Log("---------------------------------");
        }

        public SystemInformation GetDTO()
        {
            return _SystemInfo;
        }

        private void GetConnectivityInfo()
        {
            _SystemInfo.MachineName = Environment.MachineName;

            using (Socket throwawaySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                throwawaySocket.Connect("192.168.1.255", 45633);
                IPEndPoint eP = throwawaySocket.LocalEndPoint as IPEndPoint;
                _SystemInfo.IPAddress = eP.Address.ToString();
            }
        }

        private void GetOSInfo()
        {
            _SystemInfo.OSName = Environment.OSVersion.ToString();

            _SystemInfo.OSNameFriendly = new ComputerInfo().OSFullName;

            if (Environment.Is64BitOperatingSystem)
            {
                _SystemInfo.OSBitness = "x64";
            }
            else
            {
                _SystemInfo.OSBitness = "x86";
            };
        }

        private void GetResourcesInfo()
        {
            _SystemInfo.ProcessorCount = Environment.ProcessorCount;

            _SystemInfo.TotalMem = ((long)new ComputerInfo().TotalPhysicalMemory) / 1073741824.0;
        }

        private void GetMiscellaneousInfo()
        {
            _SystemInfo.UpTime = TimeSpan.FromMilliseconds((Environment.TickCount));   // minutes
        }
    }
}
