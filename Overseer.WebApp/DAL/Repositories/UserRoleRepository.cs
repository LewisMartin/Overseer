using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(OverseerDBContext context) : base(context)
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