using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class DownTimeCategoryRepository : Repository<DownTimeCategory>, IDownTimeCategoryRepository
    {
        public DownTimeCategoryRepository(OverseerDBContext context) : base(context)
        {
            // calling the base constructor
        }
    }
}