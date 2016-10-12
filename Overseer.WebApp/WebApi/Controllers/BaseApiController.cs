using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Overseer.WebApp.WebApi.Controllers
{
    public class BaseApiController : ApiController
    {
        // instantiating a private unit of work for any database work carried out within this controller
        protected readonly IUnitOfWork _unitOfWork;

        // Admin Controller Constructor - simply instantiates the UnitOfWork class (need to look into dependancy injection at some point)
        public BaseApiController()
        {
            _unitOfWork = new UnitOfWork(new OverseerDBContext());
        }
    }
}
