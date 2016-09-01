using envSeer.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace envSeer.DAL.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        // a private instance of our dbcontext
        private envSeerDBContext _dbContext;

        // instances of our repositories ()
        public IUserAuthRepository Users { get; set; }
        public IUserRoleRepository UserRoles { get; set; }

        // constructor 
        public UnitOfWork(envSeerDBContext context)
        {
            _dbContext = context;
            // using the same context across all of our repositories
            Users = new UserAuthRepository(_dbContext);
            UserRoles = new UserRoleRepository(_dbContext);
        }

        // saving changes via out dbcontext
        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        // disposing of our dbcontext
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}