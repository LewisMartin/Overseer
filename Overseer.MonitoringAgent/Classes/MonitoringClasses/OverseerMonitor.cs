using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.MonitoringClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    class OverseerMonitor : IMonitorable
    {
        public SystemInformationMonitor SysInfoMon;
        public PerformanceMonitor PerfMon;
        public DiskMonitor DiskMon;
        public EventLogMonitor EventLogMon;
        public ServiceMonitor ServiceMon;
        public ProcessMonitor ProcessMon;

        public OverseerMonitor()
        {
            // constructor - could pass initial settings to each monitoring component here..
            SysInfoMon = new SystemInformationMonitor();
            PerfMon = new PerformanceMonitor();
            DiskMon = new DiskMonitor();
            EventLogMon = new EventLogMonitor();
            ServiceMon = new ServiceMonitor();
            ProcessMon = new ProcessMonitor();
        }

        public MonitoringDataRequest GenerateMonitoringDataDTO()
        {
            return new MonitoringDataRequest()
            {

            };
        }

        public void Snapshot()
        {
            SysInfoMon.Snapshot();
            PerfMon.Snapshot();
            DiskMon.Snapshot();
            EventLogMon.Snapshot();
            ServiceMon.Snapshot();
            ProcessMon.Snapshot();
        }
    }
}
