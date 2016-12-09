using Overseer.DTOs.MonitoringAgent;
using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class EventLogMonitor : DynamicMonitor, IMonitorable<EventLogInformation>
    {
        private EventLogInformation _EventLogInfo;

        private List<EventLog> _EventLogList;

        public EventLogMonitor() { _EventLogList = new List<EventLog>(); }

        public void Snapshot()
        {
            _EventLogList.Clear();

            _EventLogInfo = new EventLogInformation();

            if (MonitoredEntities != null)
            {
                foreach (string eventLogName in MonitoredEntities)
                {
                    if (EventLog.Exists(eventLogName))
                        _EventLogList.Add(new EventLog(eventLogName));
                    else
                        _EventLogInfo.EventLogs.Add(new SingleLog() { Name = eventLogName, Exists = false });
                }
            }

            UpdateDTO();

            _Logger.Log("Snapshot successful for: Event Log");
        }

        public void LogSnapshot()
        {
            string SnapshotData = ("EVENTLOG INFO: <");

            foreach (SingleLog log in _EventLogInfo.EventLogs)
            {
                SnapshotData += String.Format(" {0}: [Exists: {1}, Display name: {2}, Total entries: {3}, Events trawled: {4}, Total infos: {5}, Total warnings: {6}, Total errors: {7}]",
                    log.Name, log.Exists, log.DisplayName, log.EntryTotal, 100, log.InfoTotal, log.WarningTotal, log.ErrorTotal);
            }
            SnapshotData += " >";

            _Logger.Log(SnapshotData);
        }

        public EventLogInformation GetDTO()
        {
            return _EventLogInfo;
        }

        private void UpdateDTO()
        {
            foreach (EventLog log in _EventLogList)
            {
                int[] logStats = ParseEventLog(log);

                _EventLogInfo.EventLogs.Add(new SingleLog()
                {
                    Name = log.Log,
                    Exists = true,
                    DisplayName = log.LogDisplayName,
                    EntryTotal = logStats[0],
                    InfoTotal = logStats[1],
                    WarningTotal = logStats[2],
                    ErrorTotal = logStats[3]
                });
            }
        }

        private int[] ParseEventLog(EventLog log)
        {
            if (log.Entries.Count > 0)
            {
                int total = log.Entries.Count, infos = 0, warnings = 0, errors = 0;

                for(int i = total-1; i > ((total-1)-((total < 100) ? total : 100)); i--)
                {
                    switch (log.Entries[i].EntryType.ToString())
                    {
                        case "Information":
                            infos++;
                            break;
                        case "Warning":
                            warnings++;
                            break;
                        case "Error":
                            errors++;
                            break;
                    }
                }

                return new int[4] { total, infos, warnings, errors };
            }
            else
            {
                return new int[4] { 0, 0, 0, 0 };
            }
        }
    }
}
