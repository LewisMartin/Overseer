using Overseer.WebApp.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Core
{
    public interface IUnitOfWork : IDisposable
    {
        // the repositories we want to enforce within our unit of work class
        // user account repos
        IUserAccountRepository Users { get; set; }
        IUserRoleRepository UserRoles { get; set; }

        // environment & machine repos
        ITestEnvironmentRepository TestEnvironments { get; set; }
        IMachineRepository Machines { get; set; }
        IMonitoringAgentCredentialRepository MonitoringAgentCredentials { get; set; }
        IOperatingSystemRepository OperatingSystems { get; set; }
        IDownTimeCategoryRepository DownTimeCategories { get; set; }

        // monitoring settings repos
        IMonitoringSettingsRepository MonitoringSettings { get; set; }
        IProcessSettingsRepository ProcessMonitoringSettings { get; set; }
        IEventLogSettingsRepository EventLogMonitoringSettings { get; set; }
        IServiceSettingsRepository ServiceMonitoringSettings { get; set; }

        // monitoring data repos
        ISystemInfoMonitoringRepository SystemInfoMonitoring { get; set; }
        IPerformanceMonitoringRepository PerformanceMonitoring { get; set; }
        IDiskMonitoringRepository DiskMonitoring { get; set; }
        IProcessMonitoringRepository ProcessMonitoring { get; set; }
        IEventLogMonitoringRepository EventLogMonitoring { get; set; }
        IServiceMonitoringRepository ServiceMonitoring { get; set; }

        // persist all entity changes across all the above repositories
        int Save();
    }
}