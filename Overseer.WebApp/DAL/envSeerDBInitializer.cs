using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Overseer.WebApp.DAL.DomainModels;

namespace Overseer.WebApp.DAL
{
    // custom database initializer class
    public class OverseerDBInitializer : DropCreateDatabaseAlways<OverseerDBContext>
    {
        // overriding the default seed method from 'DropCreateDatabaseAlways' class
        protected override void Seed(OverseerDBContext context)
        {
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

            // add the default Admin user to the db
            context.Users.Add(defaultAdmin);

            UserAccount testUser;

            // add 60 test users
            for (int i = 0; i < 60; i++)
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

            // call the base seed method (although it's my understanding that this does nothing)
            base.Seed(context);
        }
    }
}