using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.Common
{
    [Export(typeof(IAction))]
    public class TenantInvitation : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var email = request.DataStore.GetValue("InviteEmailAddress");
            var redirect = request.DataStore.GetValue("InviteRedirect");
            var inviteUrl = string.Empty;
            ActionResponse result = null;

            if(string.IsNullOrWhiteSpace(email))
            {
                result = new ActionResponse(ActionStatus.Failure, "Email address cannot be null.");
            }

            var code = GetTokenApp();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", code);
                var content = new StringContent($"{{\"invitedUserEmailAddress\":\"{email}\",\"inviteRedirectUrl\":\"{redirect}\"}}");
                var resp = client.PostAsync("https://graph.microsoft.com/v1.0/invitations", content).Result;
                if (resp.IsSuccessStatusCode)
                {
                    var respObj = JsonUtility.GetJObjectFromJsonString(resp.Content.ReadAsStringAsync().Result);
                    inviteUrl = respObj["inviteRedeemUrl"].ToString();
                    result = new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(inviteUrl));
                }
                else
                {
                    result = new ActionResponse(ActionStatus.Failure, resp.ReasonPhrase);
                }

            }

            return result;
        }

        private string GetTokenApp()
        {
            AuthenticationContext ctx = new AuthenticationContext("https://login.windows.net/" + Constants.InvitationTenant);
            var result = ctx.AcquireTokenAsync(Constants.InvitationResource, new ClientCredential(Constants.InvitationClientId, Constants.InvitationClientSecret)).Result;
            var code = result.AccessToken;
            return code;
        }

    }
}
