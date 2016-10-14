using System;

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
        // list of processes/services/eventlogs to monitor
    }

    // DTO for sending monitoring data from monitoring agent back to server
    public class MonitoringDataRequest
    {
        // all the properties that are monitored (compose this of smaller DTOs for each section - SystemInfo, PerformanceInfo, DiskInfo etc.)
    }
}