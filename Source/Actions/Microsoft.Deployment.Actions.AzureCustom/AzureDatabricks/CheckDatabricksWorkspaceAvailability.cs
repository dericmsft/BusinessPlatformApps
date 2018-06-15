using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureDatabricks
{
    [Export(typeof(IAction))]
    public class CheckDatabricksWorkspaceAvailability : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string workspaceName = request.DataStore.GetValue("DatabricksWorkspaceName");
            string location = request.DataStore.GetJson("SelectedLocation", "Name") ?? "westus";
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            dynamic payload = new ExpandoObject();
            payload.name = workspaceName;
            payload.type = "Microsoft.Databricks/workspaces";
            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);
            HttpResponseMessage response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(
                HttpMethod.Get,
                $"providers/Microsoft.Databricks/workspaces/{workspaceName}",
                "2018-04-01",
                JsonUtility.GetJsonStringFromObject(payload));

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ActionResponse(ActionStatus.Success);
            }
            else if(response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                var json = JsonUtility.GetJsonObjectFromJsonString(body);
                return new ActionResponse(ActionStatus.FailureExpected, json, null, null, json["reason"].ToString() + ": " + json["message"].ToString());
            }


            return new ActionResponse(ActionStatus.Failure, null, null, null, "Unable to query Azure for name availability");
        }
    }
}
