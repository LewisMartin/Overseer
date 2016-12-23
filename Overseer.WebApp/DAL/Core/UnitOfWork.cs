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

        // repository instances
        // user account repos
        public IUserAccountRepository Users { get; set; }
        public IUserRoleRepository UserRoles { get; set; }

        // environment & machine repos
        public ITestEnvironmentRepository TestEnvironments { get; set; }
        public IMachineRepository Machines { get; set; }
        public IMonitoringAgentCredentialRepository MonitoringAgentCredentials { get; set; }
        public IOperatingSystemRepository OperatingSystems { get; set; }
        public IDownTimeCategoryRepository DownTimeCategories { get; set; }

        // monitoring settings repos
        public IMonitoringSettingsRepository MonitoringSettings { get; set; }
        public IPerformanceSettingsRepository PerformanceMonitoringSettings { get; set; }
        public IDiskSettingsRepository DiskMonitoringSettings { get; set; }
        public IProcessSettingsRepository ProcessMonitoringSettings { get; set; }
        public IEventLogSettingsRepository EventLogMonitoringSettings { get; set; }
        public IServiceSettingsRepository ServiceMonitoringSettings { get; set; }

        // monitoring data repos
        public ISystemInfoMonitoringRepository SystemInfoMonitoring { get; set; }
        public IPerformanceMonitoringRepository PerformanceMonitoring { get; set; }
        public IDiskMonitoringRepository DiskMonitoring { get; set; }
        public IProcessMonitoringRepository ProcessMonitoring { get; set; }
        public IEventLogMonitoringRepository EventLogMonitoring { get; set; }
        public IServiceMonitoringRepository ServiceMonitoring { get; set; }

        // other
        public IMonitoringAlertRepository MonitoringAlerts { get; set; }

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
            PerformanceMonitoringSettings = new PerformanceSettingsRepository(_dbContext);
            DiskMonitoringSettings = new DiskSettingsRepository(_dbContext);
            ProcessMonitoringSettings = new ProcessSettingsRepository(_dbContext);
            EventLogMonitoringSettings = new EventLogSettingsRepository(_dbContext);
            ServiceMonitoringSettings = new ServiceSettingsRepository(_dbContext);

            SystemInfoMonitoring = new SystemInfoMonitoringRepository(_dbContext);
            PerformanceMonitoring = new PerformanceMonitoringRepository(_dbContext);
            DiskMonitoring = new DiskMonitoringRepository(_dbContext);
            ProcessMonitoring = new ProcessMonitoringRepository(_dbContext);
            EventLogMonitoring = new EventLogMonitoringRepository(_dbContext);
            ServiceMonitoring = new ServiceMonitoringRepository(_dbContext);

            MonitoringAlerts = new MonitoringAlertRepository(_dbContext);
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