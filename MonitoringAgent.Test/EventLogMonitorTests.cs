using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MonitoringAgent.Test
{
    /// <summary>
    /// Summary description for EventLogMonitorTest
    /// </summary>
    [TestClass]
    public class EventLogMonitorTests
    {
        // Arrange
        private Overseer.MonitoringAgent.MonitoringClasses.EventLogMonitor eventLogMonitor;
        private Overseer.DTOs.MonitoringAgent.EventLogInformation eventLogInfo;
        public EventLogMonitorTests()
        {
            eventLogMonitor = new Overseer.MonitoringAgent.MonitoringClasses.EventLogMonitor();
        }

        // Assert: EventLogMonitor class follows defined inheritance structure (inherits BaseMonitor & DynamicMonitor classes).
        [TestMethod]
        public void EventLog_EventLogMonitor_HasCorrectInheritance()
        {
            Assert.IsInstanceOfType(eventLogMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.BaseMonitor));
            Assert.IsInstanceOfType(eventLogMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.DynamicMonitor));
        }

        [TestMethod]
        public void EventLog_EventLogInfo_NotNull()
        {
            // Act
            eventLogMonitor.Snapshot();
            eventLogInfo = eventLogMonitor.GetDTO();

            // Assert
            Assert.IsNotNull(eventLogInfo);
        }

        [TestMethod]
        public void EventLog_EventLogs_IsEmptyGivenNoInput()
        {
            // Act
            eventLogMonitor.Snapshot();
            eventLogInfo = eventLogMonitor.GetDTO();

            // Assert
            Assert.IsTrue(eventLogInfo.EventLogs.Count == 0);
        }

        [TestMethod]
        public void EventLog_EventLogs_ContainsCorrectNumOfLogs()
        {
            // Act
            var inputList = new List<string>() { "Log1", "Log2", "Log3" };
            eventLogMonitor.UpdateMonitoredEntities(inputList);
            eventLogMonitor.Snapshot();
            eventLogInfo = eventLogMonitor.GetDTO();

            // Assert
            Assert.IsTrue(eventLogInfo.EventLogs.Count == inputList.Count);
        }
    }
}
