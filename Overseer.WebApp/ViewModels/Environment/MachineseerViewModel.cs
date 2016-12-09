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
            ProcessInfo = new ProcessInfoViewModel();
            EventLogInfo = new EventLogInfoViewModel();
            ServiceInfo = new ServicesInfoViewModel();
        }

        public SystemInfoViewModel SystemInfo { get; set; }

        public PerformanceInfoViewModel PerformanceInfo { get; set; }    // change this to a list in order to cintain multiple performance 'readings'

        public DiskInfoViewModel DiskInfo { get; set; }

        public ProcessInfoViewModel ProcessInfo { get; set; }

        public EventLogInfoViewModel EventLogInfo { get; set; }

        public ServicesInfoViewModel ServiceInfo { get; set; }
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

        public long PrivateWorkingSet { get; set; }

        public long CommitSize { get; set; }
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