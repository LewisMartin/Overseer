using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Overseer.WebApp.DAL.DomainModels;

namespace Overseer.WebApp.DAL
{
    public class OverseerDBContext : DbContext
    {
        // We're going to set it to recreate the db each time we run the website whilst we're building the model
        public OverseerDBContext() : base("name=OverseerDBContext")
        {
            Database.SetInitializer<OverseerDBContext>(new OverseerDBInitializer());
            //Database.SetInitializer<OverseerDBContext>(new CreateDatabaseIfNotExists<OverseerDBContext>());
        }

        // DbSets (tables)
        // We need to create:
        // User (Rename this to 'UserAccount' at some point)
        public DbSet<UserAccount> Users { get; set; }
        // Role (Rename this to 'UserRole' at some point)
        public DbSet<UserRole> UserRoles { get; set; }
        // Environment
        public DbSet<TestEnvironment> TestEnvironment { get; set; }
        // SupportedOS
        public DbSet<OperatingSys> SupportedOS { get; set; }
        // DownTimeCategories
        public DbSet<DownTimeCategory> DownTimeCategories { get; set; }
        // EnvironmentMonitoringSettings
        public DbSet<MonitoringSettings> MonitoringSettings { get; set; }
        // EnvPerm
        // PermLevel
        // EnvironmentCalendarTask
        // EnvironmentCalendarDeadline
        // Machine
        public DbSet<Machine> Machine { get; set; }
        // MonitoringAgentAuth
        public DbSet<MonitoringAgentCredential> MonitoringAgentCredentials { get; set; }
        //SysInfoMon
        public DbSet<SystemInfo> SystemInfoMonitoring { get; set; }
        // PerfMon
        public DbSet<PerformanceInfo> PerformanceMonitoring { get; set; }
        // DiskMon
        public DbSet<DiskInfo> DiskMonitoring { get; set; }
        // EventMon
        public DbSet<EventLogInfo> EventLogMonitoring { get; set; }
        // ServiceMon
        public DbSet<ServiceInfo> ServiceMonitoring { get; set; }
        // ProcessMon
        public DbSet<DomainModels.ProcessInfo> ProcessMonitoring { get; set; }
    }
}