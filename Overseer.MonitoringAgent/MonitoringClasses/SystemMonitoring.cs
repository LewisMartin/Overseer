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

        private DateTime _LatestSnapshot;

        private SystemInformationMonitor _SysInfoMon;
        private PerformanceMonitor _PerfMon;
        private DiskMonitor _DiskMon;
        private EventLogMonitor _EventLogMon;
        private ServiceMonitor _ServiceMon;
        private ProcessMonitor _ProcessMon;

        public SystemMonitoring()   // constructor
        {
            _Logger = Logger.Instance();

            _LatestSnapshot = new DateTime();

            _SysInfoMon = new SystemInformationMonitor();
            _PerfMon = new PerformanceMonitor();
            _DiskMon = new DiskMonitor();
            _ProcessMon = new ProcessMonitor();
            _EventLogMon = new EventLogMonitor();
            _ServiceMon = new ServiceMonitor();
        }

        public void TakeSnapshot()
        {
            _Logger.Log("Taking snapshot of current system state..");

            _SysInfoMon.Snapshot();
            _PerfMon.Snapshot();
            _DiskMon.Snapshot();
            _ProcessMon.Snapshot();
            _EventLogMon.Snapshot();
            _ServiceMon.Snapshot();

            // update time here
            _LatestSnapshot = DateTime.Now;

            _Logger.Log("Verifying obtained data..");
            _SysInfoMon.LogSnapshot();
            _PerfMon.LogSnapshot();
            _DiskMon.LogSnapshot();
            _ProcessMon.LogSnapshot();
            _EventLogMon.LogSnapshot();
            _ServiceMon.LogSnapshot();
        }

        public void UpdateMonitoringSettings(List<string> updatedProcesses, List<string> updatedEventLogs, List<string> updatedServices)
        {
            _ProcessMon.UpdateMonitoredEntities(updatedProcesses);
            _EventLogMon.UpdateMonitoredEntities(updatedEventLogs);
            _ServiceMon.UpdateMonitoredEntities(updatedServices);
        }

        // consider moving this to a 'monitoring mapper' object
        public MonitoringData GenerateMonitoringDataDTO()
        {
            return new MonitoringData()
            {
                SnapshotTime = _LatestSnapshot, // time of snapshot
                SystemInfo = _SysInfoMon.GetDTO(),
                PerformanceInfo = _PerfMon.GetDTO(),
                DiskInfo = _DiskMon.GetDTO(),
                EventLogInfo = _EventLogMon.GetDTO(),
                ServiceInfo = _ServiceMon.GetDTO(),
                ProcessInfo = _ProcessMon.GetDTO()
            };
        }
    }
}
