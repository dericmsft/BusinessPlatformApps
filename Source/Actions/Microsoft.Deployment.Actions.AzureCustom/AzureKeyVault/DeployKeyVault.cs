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
    public class DeployKeyVault : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var deploymentName = request.DataStore.GetValue("DeploymentName");
            var name = request.DataStore.GetValue("siteName");
            var objectID = request.DataStore.GetValue("objectId");
            var SPNTenantId = request.DataStore.GetValue("SPNTenantId");
            var location = "[resourceGroup().location]";
            var KeyVaultName = $"{resourceGroup}-KeyVault";

            string ArmDeploymentRelativePath = "Service/Arm/KeyVaultDeployment.json";
            string storageAccountName = "solutiontemplate" + Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("resourcegroup", resourceGroup);
            param.AddStringParam("subscription", subscription);
            param.AddStringParam("location", location);
            param.AddStringParam("KeyVaultName", KeyVaultName);
            param.AddStringParam("objectID", objectID);
            param.AddStringParam("SPNTenantId", SPNTenantId);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.ControllerModel.SiteCommonFilePath, ArmDeploymentRelativePath)));
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
                DeployedResourceType.AppServicePlan, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), string.Empty, name);

            ////Log function
            //request.Logger.LogResource(request.DataStore, name,
            //    DeployedResourceType.AppService, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), string.Empty, name);

            return new ActionResponse(ActionStatus.Success, deploymentItem);
        }
    }
}