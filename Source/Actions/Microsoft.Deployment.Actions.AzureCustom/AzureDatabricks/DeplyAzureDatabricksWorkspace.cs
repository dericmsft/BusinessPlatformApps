using Microsoft.Azure;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureDatabricks
{
    [Export(typeof(IAction))]
    public class DeplyAzureDatabricksWorkspace : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            string workspaceName = request.DataStore.GetValue("DatabricksWorkspaceName") ?? "databricksworspace-" + RandomGenerator.GetRandomLowerCaseCharacters(5);
            string location = request.DataStore.GetJson("SelectedLocation", "Name") ?? "westus";
            string sku = request.DataStore.GetValue("ASSku") ?? "B1";
            string admin = AzureUtility.GetEmailFromToken(request.DataStore.GetJson("AzureToken"));

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            AzureArmParameterGenerator param = new AzureArmParameterGenerator();
            param.AddStringParam("workspaceName", workspaceName);
            param.AddStringParam("location", location);
            param.AddStringParam("tier", "standard");

            string armTemplatefilePath = File.ReadAllText(request.ControllerModel.SiteCommonFilePath + "/service/arm/AzureDatabricks.json");
            string template = AzureUtility.GetAzureArmParameters(armTemplatefilePath, param);
            await AzureUtility.ValidateAndDeployArm(creds, resourceGroup, "DatabricksDeployment", template);
            await AzureUtility.WaitForArmDeployment(creds, resourceGroup, "DatabricksDeployment");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);
            var response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Get
                , $"/providers/Microsoft.Databricks/workspaces/{workspaceName}/"
                , "2018-04-01"
                , string.Empty
                , new Dictionary<string, string>());

            string responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure);
            }

            JObject responseObj = JsonUtility.GetJObjectFromJsonString(responseBody);
            request.DataStore.AddToDataStore("WorkspaceId", responseObj["id"], DataStoreType.Public);
            request.DataStore.AddToDataStore("WorkspaceLocation", responseObj["location"], DataStoreType.Public);
            request.DataStore.AddToDataStore("ManagedResourceGroupId", responseObj["properties"]["managedResourceGroupId"], DataStoreType.Public);

            request.Logger.LogResource(request.DataStore, responseObj["id"].ToString(),
                DeployedResourceType.AzureAnalysisServices, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), string.Empty, sku);

            return new ActionResponse(ActionStatus.Success, responseObj);
        }
    }
}
