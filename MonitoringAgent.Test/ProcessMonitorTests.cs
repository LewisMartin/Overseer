using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonitoringAgent.Test
{
    /// <summary>
    /// Summary description for ProcessMonitorTest
    /// </summary>
    [TestClass]
    public class ProcessMonitorTests
    {
        // Arrange
        private Overseer.MonitoringAgent.MonitoringClasses.ProcessMonitor processMonitor;
        private Overseer.DTOs.MonitoringAgent.ProcessInformation processInfo;
        public ProcessMonitorTests()
        {
            processMonitor = new Overseer.MonitoringAgent.MonitoringClasses.ProcessMonitor();
        }

        // Assert: ProcessMonitor class follows defined inheritance structure (inherits BaseMonitor & DynamicMonitor classes).
        [TestMethod]
        public void Process_ProcessMonitor_HasCorrectInheritance()
        {
            Assert.IsInstanceOfType(processMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.BaseMonitor));
            Assert.IsInstanceOfType(processMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.DynamicMonitor));
        }

        [TestMethod]
        public void Process_ProcessInfo_NotNull()
        {
            // Act
            processMonitor.Snapshot();
            processInfo = processMonitor.GetDTO();

            // Assert
            Assert.IsNotNull(processInfo);
        }

        [TestMethod]
        public void Process_Processes_IsEmptyGivenNoInput()
        {
            // Act
            processMonitor.Snapshot();
            processInfo = processMonitor.GetDTO();

            // Assert
            Assert.IsTrue(processInfo.Processes.Count == 0);
        }

        [TestMethod]
        public void Process_Processes_IsEmptyGivenNonExistentInputProcs()
        {
            // Act
            var inputList = new List<string>() { "NEProc1", "NEProc2", "NEProc3" };
            processMonitor.UpdateMonitoredEntities(inputList);
            processMonitor.Snapshot();
            processInfo = processMonitor.GetDTO();

            // Assert
            Assert.IsTrue(processInfo.Processes.Count == 0);
        }

        [TestMethod]
        public void Process_Processes_ContainsCorrectProcessCountGivenRealProc()
        {
            // Act
            var inputList = new List<string>();
            var existingProcName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            inputList.Add(existingProcName);
            processMonitor.UpdateMonitoredEntities(inputList);
            processMonitor.Snapshot();
            processInfo = processMonitor.GetDTO();

            // Assert
            Assert.IsTrue(processInfo.Processes.Count == (Process.GetProcessesByName(existingProcName).Length));
        }
    }
}
