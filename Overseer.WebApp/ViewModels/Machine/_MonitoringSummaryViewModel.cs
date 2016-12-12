using System;
using System.Collections.Generic;

namespace Overseer.WebApp.ViewModels.Machine
{
    public class _MonitoringSummaryViewModel
    {
        public _MonitoringSummaryViewModel()
        {
            PerformanceInfo = new PerformanceInfoViewModel();
            DiskInfo = new DiskInfoViewModel();
            ProcessInfo = new ProcessInfoViewModel();
            EventLogInfo = new EventLogInfoViewModel();
            ServiceInfo = new ServicesInfoViewModel();
        }

        public PerformanceInfoViewModel PerformanceInfo { get; set; }    // change this to a list in order to contain multiple performance 'readings'

        public DiskInfoViewModel DiskInfo { get; set; }

        public ProcessInfoViewModel ProcessInfo { get; set; }

        public EventLogInfoViewModel EventLogInfo { get; set; }

        public ServicesInfoViewModel ServiceInfo { get; set; }
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

    public class ProcessInfoViewModel
    {
        public ProcessInfoViewModel()
        {
            MonitoredProcesses = new List<ProcessGroupViewModel>();
        }

        public List<ProcessGroupViewModel> MonitoredProcesses { get; set; }
    }

    public class ProcessGroupViewModel
    {
        public ProcessGroupViewModel()
        {
            Processes = new List<SingleProcessViewModel>();
        }

        public string ProcessName { get; set; }

        public List<SingleProcessViewModel> Processes { get; set; }
    }

    public class SingleProcessViewModel
    {
        public int PID { get; set; }

        public bool Status { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan CpuTime { get; set; }

        public int ThreadCount { get; set; }

        public long WorkingSet { get; set; }

        public long PrivateBytes { get; set; }

        public long VirtualBytes { get; set; }
    }

    public class EventLogInfoViewModel
    {
        public EventLogInfoViewModel()
        {
            EventLogs = new List<SingleEventLogViewModel>();
        }

        public List<SingleEventLogViewModel> EventLogs;
    }

    public class SingleEventLogViewModel
    {
        public string EventLogName { get; set; }

        public string FriendlyLogName { get; set; }

        public bool Exists { get; set; }

        public int TotalEvents { get; set; }

        public int NumInfos { get; set; }

        public int NumWarnings { get; set; }

        public int NumErrors { get; set; }
    }

    public class ServicesInfoViewModel
    {
        public ServicesInfoViewModel()
        {
            Services = new List<SingleServiceViewModel>();
        }

        public List<SingleServiceViewModel> Services;
    }

    public class SingleServiceViewModel
    {
        public string ServiceName { get; set; }

        public bool Exists { get; set; }

        public string Status { get; set; }

        public string StartupType { get; set; }
    }
}