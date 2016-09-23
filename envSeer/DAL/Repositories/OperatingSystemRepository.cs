using envSeer.DAL.Core;
using envSeer.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace envSeer.DAL.Repositories
{
    public class OperatingSystemRepository : Repository<OperatingSys>, IOperatingSystemRepository
    {
        public OperatingSystemRepository(envSeerDBContext context) : base(context)
        {
            // calling the base constructor
        }
    }
}