using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using envSeer.Models;

namespace envSeer.DAL
{
    public class envSeerDBContext : DbContext
    {
        // We're going to set it to recreate the db each time we run whilst we're building the model
        public envSeerDBContext() : base("envSeerDBContext")
        {
            Database.SetInitializer<envSeerDBContext>(new envSeerDBInitializer());
            //Database.SetInitializer<envSeerDBContext>(new CreateDatabaseIfNotExists<envSeerDBContext>());
        }

        // DbSets (tables)
        // We need to create:
        // User
        public DbSet<UserAccount> Users { get; set; }
        // Role
        public DbSet<UserRole> UserRoles { get; set; }
        // Environment
        public DbSet<TestEnvironment> Environments { get; set; }
        // EnvPerm
        // PermLevel
        // RoadMap
        // RoadMapTask
        // Machine (or Target)
        public DbSet<Machine> Machines { get; set; }
        // PerfMon
        // DiskMon
        // EventMon
        // ServiceMon
        // ProcessMon
    }
}