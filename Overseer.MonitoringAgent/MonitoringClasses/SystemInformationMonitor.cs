using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class SystemInformationMonitor : IMonitorable
    {
        private Logger _Logger;
        
        public string MachineName { get; set; }
        public string OSName { get; set; }
        public string FriendlyOSName { get; set; }
        public string OSBitness { get; set; }
        public int ProcessorCount { get; set; }
        public double TotalMem { get; set; }
        public TimeSpan UpTime { get; set; }

        public SystemInformationMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _Logger.Log("System Information Snapshot:");

            MachineName = Environment.MachineName;

            OSName = Environment.OSVersion.ToString();

            FriendlyOSName = new ComputerInfo().OSFullName;

            if (Environment.Is64BitOperatingSystem)
            {
                OSBitness = "x64";
            }
            else
            {
                OSBitness = "x86";
            };

            ProcessorCount = Environment.ProcessorCount;

            UpTime = TimeSpan.FromMilliseconds((Environment.TickCount));   // minutes

            TotalMem = ((long)new ComputerInfo().TotalPhysicalMemory)/1073741824.0;

            _Logger.Log("Snapshot successful for: System Information");
        }

        public void DataCheck()
        {
            _Logger.Log("---------- SYSTEM INFO ----------");
            _Logger.Log("Machine Name: " + MachineName);
            _Logger.Log("OS Name: " + OSName);
            _Logger.Log("Friendly OS Name: " + FriendlyOSName);
            _Logger.Log("OS Bitness: " + OSBitness);
            _Logger.Log("Processor Count: " + ProcessorCount);
            _Logger.Log("Total RAM: " + TotalMem.ToString("0.0"));
            _Logger.Log("Current Up Time (mins): " + UpTime.ToString(@"dd\.hh\:mm\:ss"));
            _Logger.Log("---------------------------------");
        }
    }
}
