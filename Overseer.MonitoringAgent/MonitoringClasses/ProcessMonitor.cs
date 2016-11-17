using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class ProcessMonitor : DynamicMonitor, IMonitorable<ProcessInformation>
    {
        private ProcessInformation _ProcessInfo;

        public ProcessMonitor() { }

        public void Snapshot()
        {
            _ProcessInfo = new ProcessInformation();

            _Logger.Log("Snapshot successful for: Processes");
        }

        public ProcessInformation GetDTO()
        {
            return _ProcessInfo;
        }

        public void LogSnapshot()
        {
            string SnapshotData = ("PROCESS INFO: <");

            foreach (string proc in MonitoredEntities)
            {
                SnapshotData += String.Format(" {0}.exe: [IsRunning: , PID: , ThreadCount: , Status: , StartTime: , CpuTime: , MEM Usage: ]",
                    proc);
            }
            SnapshotData += " >";

            _Logger.Log(SnapshotData);
        }
    }
}
