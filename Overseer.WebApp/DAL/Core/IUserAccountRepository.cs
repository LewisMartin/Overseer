using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Core
{
    // UserAuth specific repository interface - inherits all generic opperations from generic repository interface
    public interface IUserAccountRepository : IRepository<UserAccount>
    {
        UserAccount GetWithUserRole(int userId);

        // here we declare any additional data access methods unique to the UserAccount table
        UserAccount GetUserByUsername(string name);

        // gets a range from user table - ideally this should be made generic and moved to 'IRepository' class.
        IEnumerable<UserAccount> GetRangeUsers(int startPos, int range);

        // gets all users whose usernames/full names match a search term
        IEnumerable<UserAccount> GetUserMatches(string searchTerm);

        // gets the number of users matched by the above query
        int CountUserMatches(string searchTerm);

        // gets a subset of the users returned via the above query
        IEnumerable<UserAccount> GetRangeUserMatches(int startPos, int range, string searchTerm);
    }
}