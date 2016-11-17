using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class EventLogMonitor : DynamicMonitor, IMonitorable<EventLogInformation>
    {
        private EventLogInformation EventLogInfo;

        public EventLogMonitor() { }

        public void Snapshot()
        {
            EventLogInfo = new EventLogInformation();

            _Logger.Log("Snapshot successful for: Event Log");
        }

        public EventLogInformation GetDTO()
        {
            return EventLogInfo;
        }

        public void LogSnapshot()
        {
            string SnapshotData = ("EVENTLOG INFO: <");

            foreach (string eventlog in MonitoredEntities)
            {
                SnapshotData += String.Format(" {0}: [Exists: , NumEventsTrawled: , NumInfos: , NumWarnings: , NumErrors: ]",
                    eventlog);
            }
            SnapshotData += " >";

            _Logger.Log(SnapshotData);
        }
    }
}
