using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.MonitoringClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Overseer.MonitoringAgent.Helpers;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class OverseerMonitor : IMonitorable
    {
        public SystemInformationMonitor SysInfoMon;
        public PerformanceMonitor PerfMon;
        public DiskMonitor DiskMon;
        public EventLogMonitor EventLogMon;
        public ServiceMonitor ServiceMon;
        public ProcessMonitor ProcessMon;
        private Logger _Logger;

        public OverseerMonitor()
        {
            // constructor - could pass initial settings to each monitoring component here..
            SysInfoMon = new SystemInformationMonitor();
            PerfMon = new PerformanceMonitor();
            DiskMon = new DiskMonitor();
            EventLogMon = new EventLogMonitor();
            ServiceMon = new ServiceMonitor();
            ProcessMon = new ProcessMonitor();

            _Logger = Logger.Instance();
        }

        // consider moving this to a 'monitoring mapper' object
        public MonitoringDataRequest GenerateMonitoringDataDTO()
        {
            return new MonitoringDataRequest()
            {

            };
        }

        public void Snapshot()
        {
            _Logger.Log("Taking snapshot of current system state..");

            SysInfoMon.Snapshot();
            PerfMon.Snapshot();
            DiskMon.Snapshot();
            EventLogMon.Snapshot();
            ServiceMon.Snapshot();
            ProcessMon.Snapshot();

            _Logger.Log("Verifying obtained data..");
            SysInfoMon.DataCheck();
            PerfMon.DataCheck();
            DiskMon.DataCheck();
        }
    }
}
