using Overseer.WebApp.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.DomainModels;

namespace Overseer.WebApp.DAL.Repositories
{
    // inherits from generic repository (has all generic operations), implements custom interface (forces requirement for all generic and model specific operations)
    // note here we cast the generic Repository class, hence '<UserAuth>'
    public class UserAccountRepository : Repository<UserAccount>, IUserAccountRepository
    {
        // constructor
        public UserAccountRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }

        public UserAccount GetWithUserRole(int userId)
        {
            return dbContext.Users.Include(u => u.UserRole).FirstOrDefault(u => u.UserID == userId);
        }

        // implementation of model specific operation (IUserAuthRepository)
        public UserAccount GetUserByUsername(string name)
        {
            return dbContext.Users.FirstOrDefault(u => u.UserName == name);
        }

        // gets a range from user table - ideally this should be made generic and moved to 'IRepository' class.
        public IEnumerable<UserAccount> GetRangeUsers(int startPos, int range)
        {
            return dbContext.Users.OrderBy(k => k.UserID).Skip(startPos).Take(range).ToList();
        }

        public IEnumerable<UserAccount> GetUserMatches(string searchTerm)
        {
            return dbContext.Users.Where(q => q.UserName.Contains(searchTerm) 
                                        || q.FirstName.Contains(searchTerm) 
                                        || q.LastName.Contains(searchTerm)).OrderBy(k => k.UserID).ToList();
        }

        public int CountUserMatches(string searchTerm)
        {
            return dbContext.Users.Where(q => q.UserName.Contains(searchTerm)
                            || q.FirstName.Contains(searchTerm)
                            || q.LastName.Contains(searchTerm)).OrderBy(k => k.UserID).Count();
        }

        public IEnumerable<UserAccount> GetRangeUserMatches(int startPos, int range, string searchTerm)
        {
            return dbContext.Users.Where(q => q.UserName.Contains(searchTerm)
                                        || q.FirstName.Contains(searchTerm)
                                        || q.LastName.Contains(searchTerm)).OrderBy(k => k.UserID).Skip(startPos).Take(range).ToList();
        }
    }
}