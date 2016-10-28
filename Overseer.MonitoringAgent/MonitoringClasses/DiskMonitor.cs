using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class DiskMonitor : IMonitorable<DiskInformation>
    {
        private Logger _Logger;

        private DiskInformation _DiskInfo;

        private List<DriveInfo> _DriveList { get; set; }  // Note: this .Net class cannot be serialized (as we want), hence we map each drive to a custom object

        public DiskMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _DiskInfo = new DiskInformation();

            _DriveList = DriveInfo.GetDrives().ToList();

            FilterDrives();

            UpdateDTO();

            _Logger.Log("Snapshot successful for: Disk");
        }

        public void DataCheck()
        {
            _Logger.Log("---------- DRIVE INFO ----------");
            _Logger.Log("Number of drives: " + _DriveList.Count());

            foreach (SingleDrive drive in _DiskInfo.Drives)
            {
                _Logger.Log(drive.Name + " drive (" + drive.VolumeLabel + "):");
                _Logger.Log("Drive Type: " + drive.DriveType);
                _Logger.Log("Drive Format: " + drive.DriveFormat);
                _Logger.Log("Total drive size: " + drive.TotalSpace);
                _Logger.Log("Total free space: " + drive.FreeSpace);
                _Logger.Log("Total available space: " + drive.AvailableSpace);
            }

            _Logger.Log("---------------------------------");
        }

        public DiskInformation GetDTO()
        {
            return _DiskInfo;
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
            for(int i = _DriveList.Count - 1; i >= 0; i--)
            {
                if (!_DriveList[i].IsReady)
                {
                    _DriveList.RemoveAt(i);
                }
            }
        }

        private void UpdateDTO()
        {
            _DiskInfo.DriveCount = _DriveList.Count();

            _DiskInfo.Drives = new List<SingleDrive>();

            foreach (DriveInfo drive in _DriveList)
            {
                _DiskInfo.Drives.Add(new SingleDrive()
                {
                    Name = drive.Name,
                    VolumeLabel = drive.VolumeLabel,
                    DriveType = drive.DriveType.ToString(),
                    DriveFormat = drive.DriveFormat,
                    TotalSpace = Math.Round(ConvertBytes(drive.TotalSize, "GB"), 2),
                    FreeSpace = Math.Round(ConvertBytes(drive.TotalFreeSpace, "GB"), 2),
                    AvailableSpace = Math.Round(ConvertBytes(drive.AvailableFreeSpace, "GB"), 2)
                });
            }
        }
    }
}
