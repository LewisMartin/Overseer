using Overseer.WebApp.DAL.Core;
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

        public TestEnvironment GetWithMonitoringSettings(int id)
        {
            return dbContext.TestEnvironment.Include(e => e.MonitoringSettings).FirstOrDefault(e => e.EnvironmentID == id);
        }

        public TestEnvironment GetWithAllRelatedValues(int id)
        {
            return dbContext.TestEnvironment.Include(e => e.MonitoringSettings).Include(e => e.DownTimeCategory).FirstOrDefault(e => e.EnvironmentID == id);
        }

        public TestEnvironment GetEnvironmentByCreatorAndName(int userId, string name)
        {
            return dbContext.TestEnvironment.FirstOrDefault(e => e.Creator == userId && e.EnvironmentName == name);
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