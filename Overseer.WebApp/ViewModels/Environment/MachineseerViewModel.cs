using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.ViewModels.Environment
{
    public class MachineseerViewModel
    {
        public MachineseerViewModel()
        {
            MonitoringData = new MonitoringDataViewModel();
        }

        public Guid MachineId { get; set; }

        public int ParentEnvironmentId { get; set; }

        public bool MonitoringEnabled { get; set; }

        public MachineDetailsViewModel MachineDetails { get; set; }

        public MonitoringDataViewModel MonitoringData { get; set; }
    }

    public class MonitoringDataViewModel
    {
        public MonitoringDataViewModel()
        {
            SystemInfo = new SystemInfoViewModel();
            PerformanceInfo = new PerformanceInfoViewModel();
            DiskInfo = new DiskInfoViewModel();
        }

        public SystemInfoViewModel SystemInfo { get; set; }

        public PerformanceInfoViewModel PerformanceInfo { get; set; }    // change this to a list in order to cintain multiple performance 'readings'

        public DiskInfoViewModel DiskInfo { get; set; }
    }

    public class SystemInfoViewModel
    {
        public string MachineName { get; set; }

        public string IPAddress { get; set; }

        public string OSName { get; set; }

        public string OSNameFriendly { get; set; }

        public string OSBitness { get; set; }

        public int ProcessorCount { get; set; }

        public double TotalMem { get; set; }

        public TimeSpan UpTime { get; set; }
    }

    public class PerformanceInfoViewModel
    {
        public int TotalNumProcesses { get; set; }

        public float AvgCpuUtil { get; set; }

        public float AvgMemUtil { get; set; }

        public float HighCpuUtilIndicator { get; set; }

        public float HighMemUtilIndicator { get; set; }
    }

    public class DiskInfoViewModel
    {
        public DiskInfoViewModel()
        {
            Drives = new List<SingleDriveViewModel>();
        }

        public int DriveCount { get; set; }

        public List<SingleDriveViewModel> Drives { get; set; }
    }

    public class SingleDriveViewModel
    {
        public string Name { get; set; }

        public string VolumeLabel { get; set; }

        public string DriveType { get; set; }

        public string DriveFormat { get; set; }

        public decimal TotalSpace { get; set; }

        public decimal FreeSpace { get; set; }

        public decimal AvailableSpace { get; set; }
    }
}