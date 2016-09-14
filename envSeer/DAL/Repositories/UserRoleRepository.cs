using envSeer.DAL.Core;
using envSeer.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace envSeer.DAL.Repositories
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(envSeerDBContext context) : base(context)
        {
            // calling base constructor
        }

        // method to get all the non admin user roles
        public IEnumerable<UserRole> GetNonAdminRoles()
        {
            return dbContext.UserRoles.Where(r => r.RoleName != "Administrator");
        }
    }
}