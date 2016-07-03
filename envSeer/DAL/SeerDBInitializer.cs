using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using envSeer.Models;

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
            defaultRoles.Add(new UserRole() { RoleName = "QA"});
            defaultRoles.Add(new UserRole() { RoleName = "Developer" });
            defaultRoles.Add(new UserRole() { RoleName = "Manager" });
            defaultRoles.Add(new UserRole() { RoleName = "Administrator" });

            // loop through our above UserRole entities and persist them in the UserRoles table
            foreach(UserRole defaultRole in defaultRoles)
            {
                context.UserRoles.Add(defaultRole);
            }

            // call the base seed method (although it's my understanding that this does nothing)
            base.Seed(context);
        }
    }
}