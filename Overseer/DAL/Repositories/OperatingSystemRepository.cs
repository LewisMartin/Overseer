using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class OperatingSystemRepository : Repository<OperatingSys>, IOperatingSystemRepository
    {
        public OperatingSystemRepository(OverseerDBContext context) : base(context)
        {
            // calling the base constructor
        }
    }
}