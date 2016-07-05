using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace envSeer.DAL.Core
{
    public interface IUnitOfWork : IDisposable
    {
        // the repositories we want to enforce within our unit of work class
        IUserAccountRepository Users { get; set; }

        // our save method that will persist all entity changes across all the abose repositories to the database
        int Save();
    }
}