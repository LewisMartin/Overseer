using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Overseer.WebApp.Helpers.AuthHelpers
{
    public class CustomAPIAuthAttribute : AuthorizeAttribute
    {
        protected readonly IUnitOfWork _unitOfWork;

        public CustomAPIAuthAttribute()
        {
            _unitOfWork = new UnitOfWork(new OverseerDBContext());
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            Guid TargetMachine = new Guid(actionContext.Request.Headers.GetValues("TargetMachine").FirstOrDefault());

            string TargetSecret = actionContext.Request.Headers.GetValues("TargetSecret").FirstOrDefault();

            if (AuthorizeMonitoringAgentRequest(TargetMachine, TargetSecret))
            {
                return;
                //actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
           
        }

        public bool AuthorizeMonitoringAgentRequest(Guid machineGuid, string machineSecret)
        {
            var creds = _unitOfWork.MonitoringAgentCredentials.Get(machineGuid);

            if (creds.MonitoringAgentSecret == machineSecret)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}