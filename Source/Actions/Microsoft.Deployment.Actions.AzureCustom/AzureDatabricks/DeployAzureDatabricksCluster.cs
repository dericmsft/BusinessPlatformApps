using Microsoft.Azure.Databricks.Clusters;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureDatabricks
{
    [Export(typeof(IAction))]
    class DeployAzureDatabricksCluster : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {

            var selectedLocation = request.DataStore.GetValue("SelectedLocation", "Name");
            var azureTokenDatabricks = request.DataStore.GetValue("AzureTokenDatabricks");
            var databricksClusterName = request.DataStore.GetValue("DatabricksClusterName");

            ClusterService service = new ClusterService(selectedLocation, azureTokenDatabricks);

            string clusterId = await service.Create(databricksClusterName);

            if (!string.IsNullOrWhiteSpace(clusterId))
            {
                request.DataStore.AddToDataStore("DatabricksClusterId", clusterId);
                return new ActionResponse(ActionStatus.Success);
            }
            else
            {
                return new ActionResponse(ActionStatus.Failure);
            } 
        }
    }
}
