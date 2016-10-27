using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class DiskMonitor : IMonitorable
    {
        private Logger _Logger;

        // handy .net class we can use to store our drive information
        public List<DriveInfo> Drives { get; set; }

        public DiskMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            Drives = DriveInfo.GetDrives().ToList();

            FilterDrives();

            _Logger.Log("Snapshot successful for: Disk");
        }

        public void DataCheck()
        {
            _Logger.Log("---------- DRIVE INFO ----------");
            _Logger.Log("Number of drives: " + Drives.Count());

            foreach (DriveInfo drive in Drives)
            {
                _Logger.Log(drive.Name + " drive (" + drive.VolumeLabel + "):");
                _Logger.Log("Drive Type: " + drive.DriveType);
                _Logger.Log("Drive Format: " + drive.DriveFormat);
                _Logger.Log("Total drive size: " + Math.Round(ConvertBytes(drive.TotalSize, "GB"), 2));
                _Logger.Log("Total free space: " + Math.Round(ConvertBytes(drive.TotalFreeSpace, "GB"), 2));
                _Logger.Log("Total available space: " + Math.Round(ConvertBytes(drive.AvailableFreeSpace, "GB"), 2));
            }

            _Logger.Log("---------------------------------");
        }

        private decimal ConvertBytes(long bytes, string unit)
        {
            switch (unit)
            {
                case "MB":
                    return bytes / 1024M / 1024M;
                case "GB":
                    return bytes / 1024M / 1024M / 1024M;
                case "TB":
                    return bytes / 1024M / 1024M / 1024M / 1024M;
                default:
                    return 0.0M;
            }
        }

        private void FilterDrives()
        {
            // backwards traverse list so we can remove drives safely (from the end)
            for(int i = Drives.Count - 1; i >= 0; i--)
            {
                if (!Drives[i].IsReady)
                {
                    Drives.RemoveAt(i);
                }
            }
        }
    }
}
