using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class DiskMonitor : StaticMonitor, IMonitorable<DiskInformation>
    {
        private DiskInformation _DiskInfo;

        private List<DriveInfo> _DriveList { get; set; }  // Note: this .Net class cannot be serialized (as we want), hence we map each drive to a custom object

        public DiskMonitor() { }

        public void Snapshot()
        {
            _DiskInfo = new DiskInformation();

            _DriveList = DriveInfo.GetDrives().ToList();

            FilterDrives();

            UpdateDTO();

            _Logger.Log("Snapshot successful for: Disk");
        }

        public void LogSnapshot()
        {
            string SnapshotData = String.Format("DISK INFO: <Drive Count: {0},", _DriveList.Count());

            foreach (SingleDrive drive in _DiskInfo.Drives)
            {
                SnapshotData += String.Format(" {0} drive: [Volume label: {1}, Drive type: {2}, Drive format: {3}, Total size: {4}, Free space: {5}]",
                    drive.Letter, drive.VolumeLabel, drive.DriveType, drive.DriveFormat, drive.TotalSpace, drive.FreeSpace);
            }
            SnapshotData += " >";

            _Logger.Log(SnapshotData);
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

            foreach (DriveInfo drive in _DriveList)
            {
                _DiskInfo.Drives.Add(new SingleDrive()
                {
                    Letter = drive.Name,
                    VolumeLabel = drive.VolumeLabel,
                    DriveType = drive.DriveType.ToString(),
                    DriveFormat = drive.DriveFormat,
                    TotalSpace = Math.Round(ConvertBytes(drive.TotalSize, "GB"), 2),
                    FreeSpace = Math.Round(ConvertBytes(drive.TotalFreeSpace, "GB"), 2)
                });
            }
        }
    }
}
