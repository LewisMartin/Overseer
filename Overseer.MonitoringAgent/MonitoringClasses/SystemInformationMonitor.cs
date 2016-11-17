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
    public class SystemInformationMonitor : StaticMonitor, IMonitorable<SystemInformation>
    {     
        private SystemInformation _SystemInfo;

        public SystemInformationMonitor() { }

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

        public void LogSnapshot()
        {
            string SnapshotData = String.Format("SYSTEM INFO: <Machine Name: {0}, OS Name: {1}, Friendly OS Name: {2}, OS Bitness: {3}, Processor Count: {4}, RAM: {5}, Up Time: {6}>",
                _SystemInfo.IPAddress, _SystemInfo.OSName, _SystemInfo.OSNameFriendly, _SystemInfo.OSBitness, 
                _SystemInfo.ProcessorCount, _SystemInfo.TotalMem.ToString("0.0"), _SystemInfo.UpTime.ToString(@"dd\.hh\:mm\:ss"));

            _Logger.Log(SnapshotData);
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
