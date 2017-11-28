using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureDataLake
{

    [Export(typeof(IAction))]
    class DeployADLLogicApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            string deploymentName = request.DataStore.GetValue("DeploymentName");
            string logicAppTrigger = string.Empty;
            string areaId = request.DataStore.GetValue("AxEntityDataAreaId");
            string id = request.DataStore.GetValue("AxEntityId");
            string connectionNameDataLake = request.DataStore.GetValue("ConnectorName");
            string sqlConnectionName = request.DataStore.GetValue("sqlConnectionName");
            string adlAccountName = request.DataStore.GetValue("DataLakeName");

            // Read from file
            var logicAppJsonLocation = "Service/LogicApp/adlsLogicApp.json";

            if (deploymentName == null)
            {
                deploymentName = request.DataStore.CurrentRoute;
            }

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("resourceGroup", resourceGroup);
            param.AddStringParam("subscription", subscription);
            param.AddStringParam("connectionNameDataLake", connectionNameDataLake);
            param.AddStringParam("connectionNameSql", sqlConnectionName);
            param.AddStringParam("dataLakeName", adlAccountName);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, logicAppJsonLocation)));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

            //Deploy logic app 
            var helper = new DeploymentHelper();
            var deploymentResponse = await helper.DeployLogicApp(subscription, azureToken, resourceGroup, JsonUtility.GetJObjectFromJsonString(armTemplate.ToString()), deploymentName);

            if (!deploymentResponse.IsSuccess)
            {
                return deploymentResponse;
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}
