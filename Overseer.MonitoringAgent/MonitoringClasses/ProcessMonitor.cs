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
    public class ProcessMonitor : DynamicMonitor, IMonitorable<ProcessInformation>
    {
        private ProcessInformation _ProcessInfo;

        private List<Process> _ProcessList;

        public ProcessMonitor() { _ProcessList = new List<Process>(); }

        public void Snapshot()
        {
            _ProcessInfo = new ProcessInformation();

            if (MonitoredEntities.Count() > 0)
            {
                foreach (string processName in MonitoredEntities)
                {
                    _ProcessList.AddRange(Process.GetProcessesByName(processName));
                }
            }

            UpdateDTO();

            _Logger.Log("Snapshot successful for: Processes");
        }

        public void LogSnapshot()
        {
            string SnapshotData = ("PROCESS INFO: <");

            foreach (SingleProc proc in _ProcessInfo.Processes)
            {
                SnapshotData += String.Format(" {0}.exe: [PID: {1}, Status: {2}, Start time: {3}, Cpu time: {4}, ThreadCount: {5}, Private working set: {6}, Commit size: {7}]",
                    proc.Name, proc.Pid, proc.Status, proc.StartTime, proc.CpuTime, proc.ThreadCount, (proc.PrivateWorkingSet/1024), (proc.CommitSize/2014));
            }
            SnapshotData += " >";

            _Logger.Log(SnapshotData);
        }

        public ProcessInformation GetDTO()
        {
            return _ProcessInfo;
        }

        private void UpdateDTO()
        {
            foreach (Process proc in _ProcessList)
            {
                _ProcessInfo.Processes.Add(new SingleProc()
                {
                    Name = proc.ProcessName,
                    Pid = proc.Id,
                    Status = proc.Responding,
                    StartTime = proc.StartTime,
                    CpuTime = proc.TotalProcessorTime,
                    ThreadCount = proc.Threads.Count,
                    PrivateWorkingSet = proc.WorkingSet64,
                    CommitSize = proc.PrivateMemorySize64
                });
            }
        }
    }
}
