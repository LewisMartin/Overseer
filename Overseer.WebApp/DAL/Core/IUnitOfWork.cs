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
        IUserAccountRepository Users { get; set; }
        IUserRoleRepository UserRoles { get; set; }
        ITestEnvironmentRepository TestEnvironments { get; set; }
        IMachineRepository Machines { get; set; }
        IOperatingSystemRepository OperatingSystems { get; set; }
        IDownTimeCategoryRepository DownTimeCategories { get; set; }
        IMonitoringSettingsRepository MonitoringSettings { get; set; }

        // our save method that will persist all entity changes across all the abose repositories to the database
        int Save();
    }
}