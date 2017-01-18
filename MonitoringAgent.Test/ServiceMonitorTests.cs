using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MonitoringAgent.Test
{
    [TestClass]
    public class ServiceMonitorTests
    {
        // Arrange
        private Overseer.MonitoringAgent.MonitoringClasses.ServiceMonitor serviceMonitor;
        private Overseer.DTOs.MonitoringAgent.ServiceInformation serviceInfo;
        public ServiceMonitorTests()
        {
            serviceMonitor = new Overseer.MonitoringAgent.MonitoringClasses.ServiceMonitor();
        }

        // Assert: ServiceMonitor class follows defined inheritance structure (inherits BaseMonitor & DynamicMonitor classes).
        [TestMethod]
        public void Service_ServiceMonitor_HasCorrectInheritance()
        {
            Assert.IsInstanceOfType(serviceMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.BaseMonitor));
            Assert.IsInstanceOfType(serviceMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.DynamicMonitor));
        }

        [TestMethod]
        public void Service_ServiceInfo_NotNull()
        {
            // Act
            serviceMonitor.Snapshot();
            serviceInfo = serviceMonitor.GetDTO();

            // Assert
            Assert.IsNotNull(serviceInfo);
        }

        [TestMethod]
        public void Service_ServiceInfo_IsEmptyGivenNoInput()
        {
            // Act
            serviceMonitor.Snapshot();
            serviceInfo = serviceMonitor.GetDTO();

            // Assert
            Assert.IsTrue(serviceInfo.Services.Count == 0);
        }

        [TestMethod]
        public void Service_EventLogs_ContainsCorrectNumOfLogs()
        {
            // Act
            var inputList = new List<string>() { "Service1", "Service2", "Service3" };
            serviceMonitor.UpdateMonitoredEntities(inputList);
            serviceMonitor.Snapshot();
            serviceInfo = serviceMonitor.GetDTO();

            // Assert
            Assert.IsTrue(serviceInfo.Services.Count == inputList.Count);
        }
    }
}
