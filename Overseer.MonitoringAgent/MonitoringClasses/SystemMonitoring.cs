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
    public class SystemMonitoring
    {
        private Logger _Logger;
        private SystemInformationMonitor SysInfoMon;
        private PerformanceMonitor PerfMon;
        private DiskMonitor DiskMon;
        private EventLogMonitor EventLogMon;
        private ServiceMonitor ServiceMon;
        private ProcessMonitor ProcessMon;

        public SystemMonitoring()
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

        public void TakeSnapshot()
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

        // consider moving this to a 'monitoring mapper' object
        public MonitoringData GenerateMonitoringDataDTO()
        {
            return new MonitoringData()
            {
                SystemInfo = SysInfoMon.GetDTO(),
                PerformanceInfo = PerfMon.GetDTO(),
                DiskInfo = DiskMon.GetDTO(),
                EventLogInfo = EventLogMon.GetDTO(),
                ServiceInfo = ServiceMon.GetDTO(),
                ProcessInfo = ProcessMon.GetDTO()
            };
        }
    }
}
