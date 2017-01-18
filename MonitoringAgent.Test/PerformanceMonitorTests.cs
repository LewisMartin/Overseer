using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace MonitoringAgent.Test
{
    /// <summary>
    /// Summary description for PerformanceMonitorTest
    /// </summary>
    [TestClass]
    public class PerformanceMonitorTests
    {
        // Arrange
        private Overseer.MonitoringAgent.MonitoringClasses.PerformanceMonitor perfMonitor;
        private Overseer.DTOs.MonitoringAgent.PerformanceInformation perfInfo;
        public PerformanceMonitorTests()
        {
            perfMonitor = new Overseer.MonitoringAgent.MonitoringClasses.PerformanceMonitor();
            perfMonitor.StopMonitoring();   // disable monitoring worker threads (we'll test manually)
        }

        // Assert: PerformanceMonitor class follows defined inheritance structure (inherits BaseMonitor & StaticMonitor classes).
        [TestMethod]
        public void Perf_PerformanceMonitor_HasCorrectInheritance()
        {
            Assert.IsInstanceOfType(perfMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.BaseMonitor));
            Assert.IsInstanceOfType(perfMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.StaticMonitor));
        }

        // Act
        [TestInitialize]
        public void Initialize()
        {
            Random rnd = new Random();
            for (int i = 0; i < (rnd.Next(1, 11)); i++)     // randomly take between 1 and 10 Cpu & Mem Readings
            {
                perfMonitor.AddCpuReading();
                perfMonitor.AddMemReading();
            }

            perfMonitor.Snapshot();
            perfInfo = perfMonitor.GetDTO();
        }

        // Assert: returned DTO is not null.
        [TestMethod]
        public void Perf_PerfInfo_NotNull()
        {
            Assert.IsNotNull(perfInfo);
        }

        // Assert: AvgCpuUtil value is valid %.
        [TestMethod]
        public void Perf_AvgCpuUtil_Validated()
        {
            Assert.IsNotNull(perfInfo.AvgCpuUtil);
            //Debug.WriteLine("CPU: " + perfInfo.AvgMemUtil);
            Assert.IsTrue(0 <= perfInfo.AvgCpuUtil && perfInfo.AvgCpuUtil <= 100);
        }

        // Assert: AvgMemUtil value is valid %.
        [TestMethod]
        public void Perf_AvgMemUtil_Validated()
        {
            Assert.IsNotNull(perfInfo.AvgMemUtil);
            //Debug.WriteLine("MEM: " + perfInfo.AvgMemUtil);
            Assert.IsTrue(0 <= perfInfo.AvgMemUtil && perfInfo.AvgMemUtil <= 100);
        }

        // Assert: HighCpuUtil is valid %.
        [TestMethod]
        public void Perf_HighCpuUtil_Validated()
        {
            Assert.IsNotNull(perfInfo.HighCpuUtilIndicator);
            Assert.IsTrue(0 <= perfInfo.HighCpuUtilIndicator && perfInfo.HighCpuUtilIndicator <= 100);
        }

        // Assert: HighMemUtil is valid %.
        [TestMethod]
        public void Perf_HighMemUtil_Validated()
        {
            Assert.IsNotNull(perfInfo.HighMemUtilIndicator);
            Assert.IsTrue(0 <= perfInfo.HighMemUtilIndicator && perfInfo.HighMemUtilIndicator <= 100);
        }

        // Assert: Process Count is valid.
        [TestMethod]
        public void Perf_ProcessCount_Validated()
        {
            Assert.IsTrue(perfInfo.TotalNumProcesses > 0);
        }
    }
}
