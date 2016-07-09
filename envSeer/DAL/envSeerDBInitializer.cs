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

            // save changes after adding default user
            context.SaveChanges();

            // call the base seed method (although it's my understanding that this does nothing)
            base.Seed(context);
        }
    }
}