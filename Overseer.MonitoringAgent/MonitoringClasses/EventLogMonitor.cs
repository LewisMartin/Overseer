using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class EventLogMonitor : IMonitorable<EventLogInformation>
    {
        private Logger _Logger;

        private EventLogInformation EventLogInfo;

        public EventLogMonitor()
        {
            _Logger = Logger.Instance();
        }

        public void Snapshot()
        {
            EventLogInfo = new EventLogInformation();

            _Logger.Log("Snapshot successful for: Event Log");
        }

        public EventLogInformation GetDTO()
        {
            return EventLogInfo;
        }
    }
}
