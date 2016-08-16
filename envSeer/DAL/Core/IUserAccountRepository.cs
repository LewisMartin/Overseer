﻿using envSeer.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace envSeer.DAL.Core
{
    // UserAccount specific repository interface - inherits all generic opperations from generic repository interface
    public interface IUserAccountRepository : IRepository<UserAccount>
    {
        // here we declare any additional data access methods unique to the UserAccount table
        UserAccount GetUserByUsername(string name);
    }
}