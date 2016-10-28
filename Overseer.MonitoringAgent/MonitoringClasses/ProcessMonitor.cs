using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class ProcessMonitor : IMonitorable<ProcessInformation>
    {
        private Logger _Logger;

        private ProcessInformation _ProcessInfo;

        public ProcessMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            _ProcessInfo = new ProcessInformation();

            _Logger.Log("Snapshot successful for: Processes");
        }

        public ProcessInformation GetDTO()
        {
            return _ProcessInfo;
        }
    }
}
