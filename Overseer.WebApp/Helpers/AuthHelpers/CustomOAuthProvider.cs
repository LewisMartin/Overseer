using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Overseer.WebApp.DAL.Core;
using Overseer.WebApp.DAL;
using Overseer.WebApp.DAL.DomainModels;

namespace Overseer.WebApp.Helpers.AuthHelpers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        protected readonly IUnitOfWork _unitOfWork;

        public CustomOAuthProvider()
        {
            _unitOfWork = new UnitOfWork(new OverseerDBContext());
        }

        // custom validation of POSTed credentials
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string machineGUID;
            string machineSecret;
            context.TryGetFormCredentials(out machineGUID, out machineSecret);

            Machine targetMachine = _unitOfWork.Machines.Get(new Guid(machineGUID));

            if (targetMachine != null)
            {
                if (machineSecret == "TemporarySecret")
                {
                    context.Validated(machineGUID);
                }
            }

            return base.OnValidateClientAuthentication(context);
        }

        // custom token creation
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, "MonitoringAgent"));
            var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            context.Validated(ticket);

            return base.GrantClientCredentials(context);
        }
    }
}