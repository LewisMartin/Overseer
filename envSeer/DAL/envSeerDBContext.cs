using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using envSeer.DAL.DomainModels;

namespace envSeer.DAL
{
    public class envSeerDBContext : DbContext
    {
        // We're going to set it to recreate the db each time we run the website whilst we're building the model
        public envSeerDBContext() : base("name=envSeerDBContext")
        {
            Database.SetInitializer<envSeerDBContext>(new envSeerDBInitializer());
            //Database.SetInitializer<envSeerDBContext>(new CreateDatabaseIfNotExists<envSeerDBContext>());
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
        // EnvPerm
        // PermLevel
        // RoadMap
        // RoadMapTask
        // Machine (or Target)
        public DbSet<Machine> Machine { get; set; }
        // PerfMon
        public DbSet<PerformanceMonitor> PerformanceMonitoring { get; set; }
        // DiskMon
        public DbSet<DiskMonitor> DiskMonitoring { get; set; }
        // EventMon
        public DbSet<EventLogMonitor> EventLogMonitoring { get; set; }
        // ServiceMon
        public DbSet<ServiceMonitor> ServiceMonitoring { get; set; }
        // ProcessMon
        public DbSet<ProcessMonitor> ProcessMonitoring { get; set; }
    }
}