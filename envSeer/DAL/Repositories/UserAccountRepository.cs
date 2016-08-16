using envSeer.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using envSeer.DAL;
using envSeer.DAL.DomainModels;

namespace envSeer.DAL.Repositories
{
    // inherits from generic repository (has all generic operations), implements custom interface (forces requirement for all generic and model specific operations)
    // note here we cast the generic Repository class, hence '<UserAccount>'
    public class UserAccountRepository : Repository<UserAccount>, IUserAccountRepository
    {
        // constructor
        public UserAccountRepository(envSeerDBContext context) : base(context)
        {
            // calling base constructor
        }

        // implementation of model specific operation (IUserAccountRepository)
        public UserAccount GetUserByUsername(string name)
        {
            return dbContext.Users.FirstOrDefault(u => u.UserName == name);
        }

        // gets a range from user table - ideally this should be made generic and moved to 'IRepository' class.
        public IEnumerable<UserAccount> GetRange(int startPos, int range)
        {
            return dbContext.Users.OrderBy(t => t.UserID).Skip(startPos).Take(range).ToList();
        }
    }
}