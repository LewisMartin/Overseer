using envSeer.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace envSeer.DAL.Core
{
    // UserRole specific repository interface
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        // method to get all roles apart from the admin role
        IEnumerable<UserRole> GetNonAdminRoles();
    }
}