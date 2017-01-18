using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.Devices;

namespace MonitoringAgent.Test
{
    [TestClass]
    public class SystemInformationMonitorTests
    {
        // Arrange
        private Overseer.MonitoringAgent.MonitoringClasses.SystemInformationMonitor sysInformationMonitor;
        private Overseer.DTOs.MonitoringAgent.SystemInformation sysInfo;

        // Act
        public SystemInformationMonitorTests()
        {
            sysInformationMonitor = new Overseer.MonitoringAgent.MonitoringClasses.SystemInformationMonitor();
            sysInformationMonitor.Snapshot();
            sysInfo = sysInformationMonitor.GetDTO();
        }

        // Assert: SystemInformationMonitor class follows defined inheritance structure (inherits BaseMonitor & StaticMonitor classes).
        [TestMethod]
        public void SystemInfo_SystemInformationMonitor_HasCorrectInheritance()
        {
            Assert.IsInstanceOfType(sysInformationMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.BaseMonitor));
            Assert.IsInstanceOfType(sysInformationMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.StaticMonitor));
        }

        // Assert: returned DTO is not null.
        [TestMethod]
        public void SystemInfo_SysInfo_NotNull()
        {
            Assert.IsNotNull(sysInfo);
        }

        // Assert: IPAddress is in correct format & valid.
        [TestMethod]
        public void SystemInfo_IPAddress_Validated()
        {
            bool validIP = true;

            // regex for ip address
            Regex ipRgx = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

            if (ipRgx.IsMatch(sysInfo.IPAddress))
            {
                System.Net.IPAddress temp;

                if (!System.Net.IPAddress.TryParse(sysInfo.IPAddress, out temp))
                {
                    validIP = false;
                }
            }
            else
                validIP = false;

            Assert.IsTrue(validIP);
        }

        // Assert: OS Bitness gathered by monitoring agent is consistent with environment.
        [TestMethod]
        public void SystemInfo_OSBitness_Validated()
        {
            if (Environment.Is64BitOperatingSystem)
                Assert.IsTrue(sysInfo.OSBitness == "x64");
            else
                Assert.IsTrue(sysInfo.OSBitness == "x86");
        }

        // Assert: OS Names gathered by monitoring agent are consistent with environment.
        [TestMethod]
        public void SystemInfo_OSNames_Validated()
        {
            Assert.IsNotNull(sysInfo.OSName);
            Assert.IsNotNull(sysInfo.OSNameFriendly);
            Assert.IsTrue(sysInfo.OSName == Environment.OSVersion.ToString());
            Assert.IsTrue(sysInfo.OSNameFriendly == new ComputerInfo().OSFullName);
        }

        // Assert: Machine Name gathered by monitoring agent are consistent with environment.
        [TestMethod]
        public void SystemInfo_MachineName_Validated()
        {
            Assert.IsNotNull(sysInfo.MachineName);
            Assert.IsTrue(sysInfo.MachineName == Environment.MachineName);
        }

        // Assert: Count of processors gathered by monitoring agent is consistent with environment.
        [TestMethod]
        public void SystemInfo_ProcessorCount_Validated()
        {
            Assert.IsNotNull(sysInfo.ProcessorCount);
            Assert.IsTrue(sysInfo.ProcessorCount == Environment.ProcessorCount);
        }

        // Assert: Total Memory gathered by monitoring agent is consistent with environment.
        [TestMethod]
        public void SystemInfo_TotalMem_Validated()
        {
            Assert.IsNotNull(sysInfo.TotalMem);
            Assert.IsTrue(sysInfo.TotalMem == ((long)new ComputerInfo().TotalPhysicalMemory) / 1073741824.0);
        }
    }
}
