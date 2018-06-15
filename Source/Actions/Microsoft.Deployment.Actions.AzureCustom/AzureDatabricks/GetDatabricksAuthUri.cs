using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureDatabricks
{
    [Export(typeof(IAction))]
    public class GetDatabricksAuthUri : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string workspaceName = request.DataStore.GetValue("DatabricksWorkspaceName");
            string managedResourceGroupId = request.DataStore.GetValue("ManagedResourceGroupId");

            var redirectUri = request.Info.WebsiteRootUrl + Constants.WebsiteRedirectPath;
            var authUri = $"https://westus.azuredatabricks.net/aad/auth?has=&Workspace=/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.Databricks/workspaces/{workspaceName}&WorkspaceResourceGroupUri={managedResourceGroupId}";
            //var authUri = "https://westus.azuredatabricks.net/aad/auth?has=&Workspace=/subscriptions/a72113c3-11f8-4a99-979d-70ef134cb5d5/resourceGroups/vfrordatabrickstest5-rg/providers/Microsoft.Databricks/workspaces/vfrordatabrickstest5&WorkspaceResourceGroupUri=/subscriptions/a72113c3-11f8-4a99-979d-70ef134cb5d5/resourceGroups/databricks-rg-vfrordatabrickstest5-7iyo5r4c2tc4g";
            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(authUri.ToString()));
        }
    }
}
