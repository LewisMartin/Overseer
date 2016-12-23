﻿using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Overseer.WebApp.DAL.Repositories
{
    public class TestEnvironmentRepository : Repository<TestEnvironment>, ITestEnvironmentRepository
    {
        public TestEnvironmentRepository(OverseerDBContext context) : base(context)
        {
            // calling the base constructor
        }

        public TestEnvironment GetEnvironmentAndChildMachines(int id)
        {
            return dbContext.TestEnvironment.Include(e => e.Machines).FirstOrDefault(e => e.EnvironmentID == id);
        }

        public TestEnvironment GetWithMonitoringSettings(int id)
        {
            return dbContext.TestEnvironment.Include(e => e.MonitoringSettings).FirstOrDefault(e => e.EnvironmentID == id);
        }

        public TestEnvironment GetWithAllRelatedValues(int id)
        {
            return dbContext.TestEnvironment.Include(e => e.MonitoringSettings).Include(e => e.DownTimeCategory).FirstOrDefault(e => e.EnvironmentID == id);
        }

        public TestEnvironment GetEnvironmentseerData(int id)
        {
            return dbContext.TestEnvironment
                .Include(e => e.MonitoringSettings)
                .Include(e => e.DownTimeCategory)
                .Include(e => e.Machines.Select(m => m.SystemInformationData))
                .Include(e => e.Machines.Select(m => m.OperatingSys))
                .FirstOrDefault(e => e.EnvironmentID == id);
        }

        public TestEnvironment GetEnvironmentMonitoringSummaryData(int id)
        {
            return dbContext.TestEnvironment
                .Include(e => e.Machines.Select(m => m.PerformanceData))
                .Include(e => e.Machines.Select(m => m.DiskData))
                .Include(e => e.Machines.Select(m => m.ProcessConfig))
                .Include(e => e.Machines.Select(m => m.ProcessData))
                .Include(e => e.Machines.Select(m => m.EventLogConfig))
                .Include(e => e.Machines.Select(m => m.EventLogData))
                .Include(e => e.Machines.Select(m => m.ServiceConfig))
                .Include(e => e.Machines.Select(m => m.ServiceData))
                .FirstOrDefault(e => e.EnvironmentID == id);
        }

        public bool CheckEnvironmentExistsByCreatorAndName(int userId, string name)
        {
            return dbContext.TestEnvironment.Any(e => e.Creator == userId && e.EnvironmentName == name);
        }

        public IEnumerable<TestEnvironment> GetEnvironmentsByCreator(int userId)
        {
            return dbContext.TestEnvironment.Where(c => c.Creator == userId).ToList();
        }

        public IEnumerable<TestEnvironment> GetEnvironmentsAndChildMachinesByCreator(int userId)
        {
            return dbContext.TestEnvironment.Include(c => c.Machines).Where(c => c.Creator == userId).ToList();
        }
    }
}