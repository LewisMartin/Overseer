using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Overseer.WebApp.DAL.DomainModels;

namespace Overseer.WebApp.DAL
{
    // custom database initializer class
    public class OverseerDBInitializer : DropCreateDatabaseIfModelChanges<OverseerDBContext> //DropCreateDatabaseAlways<OverseerDBContext>
    {
        // overriding the default seed method from 'DropCreateDatabaseAlways' class
        protected override void Seed(OverseerDBContext context)
        {
            // create site settigns
            SiteSetting siteSettings = new SiteSetting()
            {
                SiteID = 1,
                EnvironmentLimit = 5,
                MachineLimit = 10,
                AllowMonitoring = true,
                EnableUserRoleChange = true,
                EnableUsernameChange = true
            };
            context.SiteSettings.Add(siteSettings);
            context.SaveChanges();

            // Create a list of default user roles
            List<UserRole> defaultRoles = new List<UserRole>();

            // add the default roles to the list
            defaultRoles.Add(new UserRole() { RoleName = "Administrator" });
            defaultRoles.Add(new UserRole() { RoleName = "QA"});
            defaultRoles.Add(new UserRole() { RoleName = "Developer" });
            defaultRoles.Add(new UserRole() { RoleName = "Manager" });

            // loop through our above UserRole entities and persist them in the UserRoles table
            foreach(UserRole defaultRole in defaultRoles)
            {
                context.UserRoles.Add(defaultRole);
            }
            // save changes after adding roles
            context.SaveChanges();

            
            // instantiate our hashing provider
            var crypto = new SimpleCrypto.PBKDF2();
            crypto.GenerateSalt();

            // create default admin account
            UserAccount defaultAdmin = new UserAccount();
            defaultAdmin.UserName = "Admin";
            defaultAdmin.FirstName = "Admin";
            defaultAdmin.LastName = "Admin";
            defaultAdmin.UserRoleID = Int32.Parse("1");
            defaultAdmin.Email = "admin@Overseer.WebApp.local";
            defaultAdmin.Password = crypto.Compute("W3lcome");
            defaultAdmin.PasswordSalt = crypto.Salt;
            context.Users.Add(defaultAdmin);
            context.SaveChanges();

            // create default QA account
            UserAccount defaultQA = new UserAccount();
            defaultQA.UserName = "MHarrington";
            defaultQA.FirstName = "Michael";
            defaultQA.LastName = "Harrington";
            defaultQA.UserRoleID = Int32.Parse("2");
            defaultQA.Email = "mikeharrington@Overseer.WebApp.local";
            defaultQA.Password = crypto.Compute("W3lcome");
            defaultQA.PasswordSalt = crypto.Salt;
            context.Users.Add(defaultQA);
            context.SaveChanges();

            // create default Developer account
            UserAccount defaultDev = new UserAccount();
            defaultDev.UserName = "AnsonCheung";
            defaultDev.FirstName = "Anson";
            defaultDev.LastName = "Cheung";
            defaultDev.UserRoleID = Int32.Parse("3");
            defaultDev.Email = "anson@Overseer.WebApp.local";
            defaultDev.Password = crypto.Compute("W3lcome");
            defaultDev.PasswordSalt = crypto.Salt;
            context.Users.Add(defaultAdmin);
            context.SaveChanges();

            // create default Manager account
            UserAccount defaultMan = new UserAccount();
            defaultMan.UserName = "JaneDoe";
            defaultMan.FirstName = "Jane";
            defaultMan.LastName = "Doe";
            defaultMan.UserRoleID = Int32.Parse("4");
            defaultMan.Email = "janedoe@Overseer.WebApp.local";
            defaultMan.Password = crypto.Compute("W3lcome");
            defaultMan.PasswordSalt = crypto.Salt;
            context.Users.Add(defaultMan);
            context.SaveChanges();

            UserAccount testUser;
            // add 60 test users
            for (int i = 0; i < 10; i++)
            {
                testUser = new UserAccount();

                testUser.UserName = "TestUser" + i.ToString();
                testUser.FirstName = "Test";
                testUser.LastName = "User " + i.ToString();
                testUser.UserRoleID = Int32.Parse("2");
                testUser.Email = "TestUser" + i.ToString() + "@Overseer.WebApp.local";
                testUser.Password = crypto.Compute("password");
                testUser.PasswordSalt = crypto.Salt;

                context.Users.Add(testUser);
            }
            // save changes after adding default user
            context.SaveChanges();

            // seeding the SupportedOS table:
            List<OperatingSys> supportedOperatingSystems = new List<OperatingSys> {

                new OperatingSys() { OSName = "Windows 10 x64", Bitness = 64 },
                new OperatingSys() { OSName = "Windows 10 x86", Bitness = 32 },
                new OperatingSys() { OSName = "Windows 8 x64", Bitness = 64 },
                new OperatingSys() { OSName = "Windows 8 x86", Bitness = 32 },
                new OperatingSys() { OSName = "Windows Server 2012", Bitness = 64 },
                new OperatingSys() { OSName = "Windows Server 2012 R2", Bitness = 64 },
                new OperatingSys() { OSName = "Windows Server 2016", Bitness = 64 }
            };

            // loop through our above UserRole entities and persist them in the UserRoles table
            foreach (OperatingSys OS in supportedOperatingSystems)
            {
                context.SupportedOS.Add(OS);
            }
            // save changes after adding supported Operating Systems
            context.SaveChanges();


            // seeding the DownTimeCategory table:
            List<DownTimeCategory> downTimeCats = new List<DownTimeCategory>
            {
                new DownTimeCategory() { Name = "Configuration", Desc = "Configuration changes are being made to machines within this environment." },
                new DownTimeCategory() { Name = "Scheduled Maintenance", Desc = "Scheduled maintenance is being carried out on machines within this environment." },
                new DownTimeCategory() { Name = "Hardware Failure", Desc = "Hardware failure has occured on one or more machines within this environment." },
                new DownTimeCategory() { Name = "Network Issues", Desc = "Network issues are preventing connections being made to one or more machines within this environment." },
                new DownTimeCategory() { Name = "Unused/Power Saving", Desc = "There are currently no testing activities taking place within this environment - shut down for power saving." }
            };

            // add all above categories to table
            foreach (DownTimeCategory downTimeCat in downTimeCats)
            {
                context.DownTimeCategories.Add(downTimeCat);
            }
            // save changes after adding down time categories
            context.SaveChanges();

            // create default example environment (owned by admin)
            TestEnvironment exampleEnvironment = new TestEnvironment();
            exampleEnvironment.EnvironmentID = 1;
            exampleEnvironment.EnvironmentName = "Example Environment";
            exampleEnvironment.Creator = 1;
            exampleEnvironment.IsPrivate = false;
            exampleEnvironment.Status = true;
            context.TestEnvironment.Add(exampleEnvironment);
            context.SaveChanges();

            MonitoringSettings exampleEnvMonSettings = new MonitoringSettings()
            {
                EnvironmentID = 1,
                MonitoringEnabled = false,
                MonitoringUpdateInterval = 5,
                MonitoringUpdateSchedule = 5
            };
            context.MonitoringSettings.Add(exampleEnvMonSettings);
            context.SaveChanges();

            // call the base seed method (although it's my understanding that this does nothing)
            base.Seed(context);
        }
    }
}