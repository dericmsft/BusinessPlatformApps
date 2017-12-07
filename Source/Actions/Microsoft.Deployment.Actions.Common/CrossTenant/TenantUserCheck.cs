using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using System.Net.Http;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Actions.Common.CrossTenant
{
    [Export(typeof(IAction))]
    public class TenantUserCheck : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var email = request.DataStore.GetValue("InviteEmailAddress");

            if (string.IsNullOrEmpty(email))
            {
                return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue("false"));
            }

            var code = TenantHelpers.GetTenantToken();
            ActionResponse result = new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue("false"));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", code);

                var resp = client.GetAsync("https://graph.microsoft.com/v1.0/users").Result;
                if (resp.IsSuccessStatusCode)
                {
                    var respObj = JsonUtility.GetJObjectFromJsonString(resp.Content.ReadAsStringAsync().Result);
                    foreach (var entry in respObj["value"])
                    {
                        if (entry["userPrincipalName"].ToString().Contains(email.Split('@').First()))
                        {
                            result = new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue("true"));
                            break;
                        }
                    }
                }
                else
                {
                    result = new ActionResponse(ActionStatus.Failure, resp.ReasonPhrase);
                }

            }
            return result;
        }
    }
}
