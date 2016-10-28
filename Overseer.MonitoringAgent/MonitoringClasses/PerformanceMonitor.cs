using Overseer.MonitoringAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Overseer.DTOs.MonitoringAgent;

namespace Overseer.MonitoringAgent.MonitoringClasses
{
    public class PerformanceMonitor : IMonitorable<PerformanceInformation>
    {
        private Logger _Logger;

        private PerformanceInformation _PerformanceInfo;

        private readonly Object _Lock_CpuHighUtilCounter = new Object();
        private readonly Object _Lock_MemHighUtilCounter = new Object();

        private PerformanceCounter _CpuUtilCounter;
        private PerformanceCounter _MemUtilCounter;
        private List<float> _CpuReadings = new List<float>();
        private List<float> _MemReadings = new List<float>();
        private int _CpuHighUtilCounter;
        private int _MemHighUtilCounter;

        public PerformanceMonitor()
        {
            _Logger = Logger.Instance();
            _CpuUtilCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _MemUtilCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");

            BeginMonitoring();
        }

        public void Snapshot()
        {
            _PerformanceInfo = new PerformanceInformation();

            Process[] AllProcesses = Process.GetProcesses();

            _PerformanceInfo.TotalNumProcesses = AllProcesses.Count();

            SnapshotCpu();
            SnapshotMem();

            ResetReadings();

            _Logger.Log("Snapshot successful for: Performance");
        }

        public void DataCheck()
        {
            _Logger.Log("------- PERFORMANCE INFO -------");
            _Logger.Log("Number of processes: " + _PerformanceInfo.TotalNumProcesses);
            _Logger.Log("Avg CPU Usage: " + _PerformanceInfo.AvgCpuUtil + "%");
            _Logger.Log("High CPU Usage: " + _PerformanceInfo.HighCpuUtilIndicator + "%");
            _Logger.Log("Avg MEM Usage: " + _PerformanceInfo.AvgMemUtil + "%");
            _Logger.Log("High MEM Usage: " + _PerformanceInfo.HighMemUtilIndicator + "%");
            _Logger.Log("---------------------------------");
        }

        public PerformanceInformation GetDTO()
        {
            return _PerformanceInfo;
        }

        private void BeginMonitoring()
        {
            _CpuHighUtilCounter = 0;
            _MemHighUtilCounter = 0;

            Thread CpuMonitoringWorker = new Thread(CpuMonitoring);
            Thread MemMonitoringWorker = new Thread(MemMonitoring);

            CpuMonitoringWorker.Start();
            MemMonitoringWorker.Start();
        }

        private void CpuMonitoring()
        {
            _Logger.Log("Starting cpu monitoring..");

            while (true)
            {
                lock (_CpuReadings)
                {
                    _CpuReadings.Add(_CpuUtilCounter.NextValue());

                    if ((_CpuReadings[_CpuReadings.Count() - 1]) >= 80)
                    {
                        lock (_Lock_CpuHighUtilCounter)
                        {
                            _CpuHighUtilCounter++;
                        }
                    }
                }

                Thread.Sleep(3000);
            }
        }

        private void MemMonitoring()
        {
            _Logger.Log("Starting memory monitoring..");

            while (true)
            {
                lock (_MemReadings)
                {
                    _MemReadings.Add(_MemUtilCounter.NextValue());

                    if ((_MemReadings[_MemReadings.Count() - 1]) >= 80)
                    {
                        lock (_Lock_MemHighUtilCounter)
                        {
                            _MemHighUtilCounter++;
                        }
                    }
                }

                Thread.Sleep(3000);
            }
        }

        private void SnapshotCpu()
        {
            lock (_CpuReadings) lock (_Lock_CpuHighUtilCounter)
                {
                    CalcAvgCpuUtil();
                    CalcHighCpuUsage();
                }

            _Logger.Log("Cpu statistics snapshotted.");
        }

        private void SnapshotMem()
        {
            lock (_MemReadings) lock (_Lock_MemHighUtilCounter)
                {
                    CalcAvgMemUtil();
                    CalcHighMemUsage();
                }

            _Logger.Log("Mem statistics snapshotted.");
        }

        private void CalcAvgCpuUtil()
        {
            float total = 0;

            foreach (float cpuReading in _CpuReadings)
            {
                total += cpuReading;
            }

            _PerformanceInfo.AvgCpuUtil = total / (_CpuReadings.Count());
        }

        private void CalcAvgMemUtil()
        {
            float total = 0;

            foreach (float memReading in _MemReadings)
            {
                total += memReading;
            }

            _PerformanceInfo.AvgMemUtil = total / (_MemReadings.Count());
        }

        private void CalcHighCpuUsage()
        {
            _PerformanceInfo.HighCpuUtilIndicator = ((float)_CpuHighUtilCounter / (float)_CpuReadings.Count()) * 100;
        }

        private void CalcHighMemUsage()
        {
            _PerformanceInfo.HighMemUtilIndicator = ((float)_MemHighUtilCounter / (float)_MemReadings.Count()) * 100;
        }

        private void ResetReadings()
        {
            lock (_CpuReadings)
            {
                _CpuReadings.Clear();
            }

            lock (_MemReadings)
            {
                _MemReadings.Clear();
            }

            lock (_Lock_CpuHighUtilCounter)
            {
                _CpuHighUtilCounter = 0;
            }

            lock (_Lock_MemHighUtilCounter)
            {
                _MemHighUtilCounter = 0;
            }
        }
    }
}
