using Overseer.DTOs.MonitoringAgent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class ProcessMonitor : DynamicMonitor, IMonitorable<ProcessInformation>
    {
        private ProcessInformation _ProcessInfo;

        private List<Process> _ProcessList;

        public ProcessMonitor() { _ProcessList = new List<Process>(); }

        public void Snapshot()
        {
            _ProcessList.Clear();

            _ProcessInfo = new ProcessInformation();

            if (MonitoredEntities != null)
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

            foreach (SingleProc procDat in _ProcessInfo.Processes)
            {
                SnapshotData += String.Format(" {0}.exe: [PID: {1}, Status: {2}, Start time: {3}, Cpu time: {4}, ThreadCount: {5}, Private working set: {6}, Commit size: {7}]",
                    procDat.Name, procDat.Pid, procDat.Status, procDat.StartTime, procDat.CpuTime, procDat.ThreadCount, (procDat.WorkingSet/1024), (procDat.PrivateBytes/1024), (procDat.VirtualBytes/2014));
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
                    WorkingSet = proc.WorkingSet64,             // Equal to: 'Working Set' performance counter.
                    PrivateBytes = proc.PrivateMemorySize64,    // Equal to: 'Private Bytes' performance counter.
                    VirtualBytes = proc.VirtualMemorySize64     // Equal to: 'Virtual Bytes' performance counter.
                });
            }
        }
    }
}
