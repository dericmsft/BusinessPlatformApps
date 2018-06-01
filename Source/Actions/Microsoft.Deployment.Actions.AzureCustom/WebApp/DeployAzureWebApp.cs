﻿using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployAzureWebApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            var deploymentName = request.DataStore.GetValue("DeploymentName");
            var name = request.DataStore.GetValue("siteName");
            //var hostingPlanName = request.DataStore.GetValue("hostingPlanName") ?? "apphostingplan";
            
            var sku = "S1";
            var workerSize = "1";
            var repoURL = "https://github.com/v-preben/Demo.git";
            var branch = "Test";
            var location = "[resourceGroup().location]";

            //var hostingEnvironment = request.DataStore.GetValue("hostingEnvironment") ?? string.Empty;
            //var sku = request.DataStore.GetValue("sku") ?? "S1";
            //var skuCode = request.DataStore.GetValue("skuCode") ?? "S1";
            //var workerSize = request.DataStore.GetValue("workerSize") ?? "0";
            //var branch = request.DataStore.GetValue("branch") ?? "master";
            /*var projectPath = request.DataStore.GetValue("ProjectPath") ?? string.Empty; Commented out until we find a way to hasten deployments -we will bring back functions to the same repo then */


            //string functionArmDeploymentRelativePath = sku.ToLower() == "standard"
            //     ? "Service/Arm/AzureFunctionsStaticAppPlan.json"
            //     : "Service/Arm/AzureFunctions.json";
            string webAppArmDeploymentRelativePath = "Service/Arm/AzureWebApp.json";

            string storageAccountName = "solutiontemplate" + Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("resourcegroup", resourceGroup);
            param.AddStringParam("subscription", subscription);
            param.AddStringParam("siteName", name);
            param.AddStringParam("hostingPlanName", name);
            param.AddStringParam("sku", sku);
            param.AddStringParam("workerSize", workerSize);
            param.AddStringParam("repoURL", repoURL);
            param.AddStringParam("branch", branch);
            param.AddStringParam("location", location);
            //param.AddStringParam("hostingEnvironment", hostingEnvironment);
            //param.AddStringParam("sku", sku);
            //param.AddStringParam("skuCode", skuCode);
            //param.AddStringParam("workerSize", workerSize);
            //param.AddStringParam("branch", branch);
            /*param.AddStringParam("projectPath", projectPath); Commented out until we find a way to hasten deployments - we will bring back functions to the same repo then*/

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.ControllerModel.SiteCommonFilePath, webAppArmDeploymentRelativePath)));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            ResourceManagementClient client = new ResourceManagementClient(creds);


            var deployment = new Azure.Management.Resources.Models.Deployment()
            {
                Properties = new DeploymentPropertiesExtended()
                {
                    Template = armTemplate.ToString(),
                    Parameters = JsonUtility.GetEmptyJObject().ToString()
                }
            };

            var validate = await client.Deployments.ValidateAsync(resourceGroup, deploymentName, deployment, new CancellationToken());
            if (!validate.IsValid)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(validate), null,
                     DefaultErrorCodes.DefaultErrorCode, $"Azure:{validate.Error.Message} Details:{validate.Error.Details}");
            }

            var deploymentItem = await client.Deployments.CreateOrUpdateAsync(resourceGroup, deploymentName, deployment, new CancellationToken());

            //Log app hosting plan
            request.Logger.LogResource(request.DataStore, name,
                DeployedResourceType.AppServicePlan, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), string.Empty,name );

            //Log function
            request.Logger.LogResource(request.DataStore, name,
                DeployedResourceType.Function, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), string.Empty, name);

            //Log storage account
            request.Logger.LogResource(request.DataStore, storageAccountName,
                DeployedResourceType.StorageAccount, CreatedBy.BPST, DateTime.UtcNow.ToString("o"));

            return new ActionResponse(ActionStatus.Success, deploymentItem);
        }
    }
}