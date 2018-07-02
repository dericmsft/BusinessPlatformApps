using System;
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
			// Fetching values from the datastore
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");            
            var deploymentName = request.DataStore.GetValue("DeploymentName");
			var name = request.DataStore.GetValue("webAppName") ;
            var repoURL = request.DataStore.GetValue("RepoURL");
            var branch = request.DataStore.GetValue("Branch");
            var project = request.DataStore.GetValue("Project");
            var armTemplateName = request.DataStore.GetValue("ArmTemplateName");                        
            var location = "[resourceGroup().location]";            
            
            string webAppArmDeploymentRelativePath = "Service/Arm/" + armTemplateName +".json";
            string storageAccountName = "solutiontemplate" + Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

			// Attaching parameters to the ARM Template
            var param = new AzureArmParameterGenerator();
            param.AddStringParam("resourcegroup", resourceGroup);
            param.AddStringParam("subscription", subscription);
            param.AddStringParam("siteName", name);
            param.AddStringParam("hostingPlanName", name);
            param.AddStringParam("project", project);		
            param.AddStringParam("repoURL", repoURL);
            param.AddStringParam("branch", branch);
            param.AddStringParam("location", location);
            
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
                DeployedResourceType.AppService, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), string.Empty, name);

            return new ActionResponse(ActionStatus.Success, deploymentItem);
        }
    }
}