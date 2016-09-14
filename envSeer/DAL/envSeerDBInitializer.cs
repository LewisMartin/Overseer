using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using envSeer.DAL.DomainModels;

namespace envSeer.DAL
{
    // custom database initializer class
    public class envSeerDBInitializer : DropCreateDatabaseAlways<envSeerDBContext>
    {
        // overriding the default seed method from 'DropCreateDatabaseAlways' class
        protected override void Seed(envSeerDBContext context)
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
            defaultAdmin.Email = "admin@envseer.local";
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
                testUser.Email = "TestUser" + i.ToString() + "@envseer.local";
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

            // call the base seed method (although it's my understanding that this does nothing)
            base.Seed(context);
        }
    }
}