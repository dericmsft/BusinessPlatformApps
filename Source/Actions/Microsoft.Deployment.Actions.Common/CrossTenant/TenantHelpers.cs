using Microsoft.Deployment.Common;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.Common.CrossTenant
{
    public static class TenantHelpers
    {
        public static string GetTenantToken()
        {
            AuthenticationContext ctx = new AuthenticationContext("https://login.windows.net/" + Constants.InvitationTenant);
            var result = ctx.AcquireTokenAsync(Constants.InvitationResource, new ClientCredential(Constants.InvitationClientId, Constants.InvitationClientSecret)).Result;
            var code = result.AccessToken;
            return code;
        }
    }
}
