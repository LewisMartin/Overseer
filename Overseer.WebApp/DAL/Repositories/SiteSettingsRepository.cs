using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Overseer.WebApp.DAL.Repositories
{
    public class SiteSettingsRepository : Repository<SiteSetting>, ISiteSettingsRepository
    {
        public SiteSettingsRepository(OverseerDBContext context) : base(context)
        {
            // calling base constructor
        }
    }
}