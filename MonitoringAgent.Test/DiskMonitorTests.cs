using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace MonitoringAgent.Test
{
    /// <summary>
    /// Summary description for DiskMonitorTest
    /// </summary>
    [TestClass]
    public class DiskMonitorTests
    {
        // Arrange
        private Overseer.MonitoringAgent.MonitoringClasses.DiskMonitor diskMonitor;
        private Overseer.DTOs.MonitoringAgent.DiskInformation diskInfo;

        // Act
        public DiskMonitorTests()
        {
            diskMonitor = new Overseer.MonitoringAgent.MonitoringClasses.DiskMonitor();
            diskMonitor.Snapshot();
            diskInfo = diskMonitor.GetDTO();
        }

        // Assert: DiskMonitor class follows defined inheritance structure (inherits BaseMonitor & StaticMonitor classes).
        [TestMethod]
        public void Disk_DiskMonitor_HasCorrectInheritance()
        {
            Assert.IsInstanceOfType(diskMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.BaseMonitor));
            Assert.IsInstanceOfType(diskMonitor, typeof(Overseer.MonitoringAgent.MonitoringClasses.StaticMonitor));
        }

        // Assert: returned DTO is not null.
        [TestMethod]
        public void Disk_DiskInfo_NotNull()
        {
            Assert.IsNotNull(diskInfo);
        }

        // Assert: Number of drives reported is valid & consistent.
        [TestMethod]
        public void Disk_DriveCount_Validated()
        {
            // ensure number of drives is at least one (none of our targets can run without a drive attached)
            Assert.IsTrue(diskInfo.DriveCount > 0);

            // ensure value stored in 'DriveCount' actually reflects the number of drives that dta has been collected on
            Assert.IsTrue(diskInfo.Drives.Count == diskInfo.DriveCount);
        }

        // Assert: Drive letter for each monitored drive is valid.
        [TestMethod]
        public void Disk_DriveLetters_Validated()
        {
            // regex for disk letter - e.g. 'C:\'
            Regex rgx = new Regex(@"[a-zA-Z](:)(\\)");

            foreach (var drive in diskInfo.Drives)
            {
                Assert.IsTrue(rgx.IsMatch(drive.Letter));
            }
        }

        // Assert: Total/Free space for each drive is valid & consistent.
        [TestMethod]
        public void Disk_DriveSpace_Validated()
        {
            foreach (var drive in diskInfo.Drives)
            {
                Assert.IsTrue(drive.TotalSpace >= drive.FreeSpace);  // total space is never smaller than free space
                Assert.IsTrue(drive.FreeSpace >= 0);    // free space is never negative
            }
        }
    }
}
