using System;
using System.Collections.Generic;

namespace Overseer.DTOs.MonitoringAgent
{
    // DTO for returning monitoring scheduling data to monitoring agent
    public class MonitoringScheduleResponse
    {
        public bool MonitoringEnabled { get; set; }

        public DateTime NextScheduledUpdate { get; set; }
    }

    // DTO for returning monitoring settings to monitoring agent
    public class MonitoringSettingsResponse
    {
        public MonitoringSettingsResponse()
        {
            MonitoredProcessNames = new List<string>();
            MonitoredEventLogNames = new List<string>();
            MonitoredServiceNames = new List<string>();
        }

        // list of processes/services/eventlogs to monitor
        public List<string> MonitoredProcessNames { get; set; }
        public List<string> MonitoredEventLogNames { get; set; }
        public List<string> MonitoredServiceNames { get; set; }
    }

    // DTO for sending monitoring data from monitoring agent back to server
    public class MonitoringData
    {
        // all the properties that are monitored (compose this of smaller DTOs for each section - SystemInfo, PerformanceInfo, DiskInfo etc.)
        public SystemInformation SystemInfo;
        public PerformanceInformation PerformanceInfo;
        public DiskInformation DiskInfo;
        public EventLogInformation EventLogInfo;
        public ServiceInformation ServiceInfo;
        public ProcessInformation ProcessInfo;
    }

    // above is comprised of the below 
    public class SystemInformation : MonitoredInformation
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
    public class PerformanceInformation : MonitoredInformation
    {
        public int TotalNumProcesses { get; set; }
        public float AvgCpuUtil { get; set; }
        public float AvgMemUtil { get; set; }
        public float HighCpuUtilIndicator { get; set; }
        public float HighMemUtilIndicator { get; set; }
    }
    public class DiskInformation : MonitoredInformation
    {
        public int DriveCount;
        public List<SingleDrive> Drives;
    }
    public struct SingleDrive
    {
        public string Name;
        public string VolumeLabel;
        public string DriveType;
        public string DriveFormat;
        public decimal TotalSpace;
        public decimal FreeSpace;
        public decimal AvailableSpace;
    }
    public class EventLogInformation : MonitoredInformation
    {

    }
    public class ServiceInformation : MonitoredInformation
    {

    }
    public class ProcessInformation : MonitoredInformation
    {

    }
    public class MonitoredInformation
    {

    }
}