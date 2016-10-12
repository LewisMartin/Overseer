using Overseer.WebApp.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        // a private instance of our dbcontext
        private OverseerDBContext _dbContext;

        // instances of our repositories ()
        public IUserAccountRepository Users { get; set; }
        public IUserRoleRepository UserRoles { get; set; }
        public ITestEnvironmentRepository TestEnvironments { get; set; }
        public IMachineRepository Machines { get; set; }
        public IMonitoringAgentCredentialRepository MonitoringAgentCredentials { get; set; }
        public IOperatingSystemRepository OperatingSystems { get; set; }
        public IDownTimeCategoryRepository DownTimeCategories { get; set; }
        public IMonitoringSettingsRepository MonitoringSettings { get; set; }

        // constructor 
        public UnitOfWork(OverseerDBContext context)
        {
            _dbContext = context;
            // using the same context across all of our repositories
            Users = new UserAccountRepository(_dbContext);
            UserRoles = new UserRoleRepository(_dbContext);
            TestEnvironments = new TestEnvironmentRepository(_dbContext);
            Machines = new MachineRepository(_dbContext);
            MonitoringAgentCredentials = new MonitoringAgentCredentialRepository(_dbContext);
            OperatingSystems = new OperatingSystemRepository(_dbContext);
            DownTimeCategories = new DownTimeCategoryRepository(_dbContext);
            MonitoringSettings = new MonitoringSettingsRepository(_dbContext);
        }

        // saving changes via out dbcontext
        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        // disposing of our dbcontext
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}