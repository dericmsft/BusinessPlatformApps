using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure;
using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureAS
{
    [Export(typeof(IAction))]
    public class DeployAzureAnalysisServices : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureTokenAS", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            string serverName = request.DataStore.GetValue("ASServerName") ?? "analysisserver-" + RandomGenerator.GetRandomLowerCaseCharacters(5);
            string location = request.DataStore.GetValue("ASLocation") ?? "westus2";
            string sku = request.DataStore.GetValue("ASSku") ?? "B1";
            string admin = AzureUtility.GetEmailFromToken(request.DataStore.GetJson("AzureToken"));

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            AzureArmParameterGenerator param = new AzureArmParameterGenerator();
            param.AddStringParam("name", serverName);
            param.AddStringParam("location", location);
            param.AddStringParam("sku", sku);
            param.AddStringParam("admin", admin);

            string armTemplatefilePath = File.ReadAllText(request.ControllerModel.SiteCommonFilePath + "/service/arm/NewAzureAS.json");
            string template = AzureUtility.GetAzureArmParameters(armTemplatefilePath, param);
            await AzureUtility.ValidateAndDeployArm(creds, resourceGroup, "ASDeployment", template);
            await AzureUtility.WaitForArmDeployment(creds, resourceGroup, "ASDeployment");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);
            var response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Get
                , $"/providers/Microsoft.AnalysisServices/servers/{serverName}/"
                , "2016-05-16"
                , string.Empty
                , new Dictionary<string, string>());

            string responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure);
            }

            JObject responseObj = JsonUtility.GetJObjectFromJsonString(responseBody);
            request.DataStore.AddToDataStore("ASServerUrl", responseObj["properties"]["serverFullName"], DataStoreType.Public);
			request.DataStore.AddToDataStore("ASServerName", serverName, DataStoreType.Public);

            request.Logger.LogResource(request.DataStore, responseObj["properties"]["serverFullName"].ToString(),
                DeployedResourceType.AzureAnalysisServices, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), string.Empty, sku);

            return new ActionResponse(ActionStatus.Success, responseObj);
        }
    }
}