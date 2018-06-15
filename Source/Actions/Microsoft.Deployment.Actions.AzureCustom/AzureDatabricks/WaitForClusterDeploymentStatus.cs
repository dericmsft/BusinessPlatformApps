using Microsoft.Azure.Databricks.Clusters;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureDatabricks
{
    [Export(typeof(IAction))]
    public class WaitForClusterDeploymentStatus : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var selectedLocation = request.DataStore.GetJson("SelectedLocation", "Name");
            var azureTokenDatabricks = request.DataStore.GetValue("AzureTokenDatabricks");
            var databricksClusterId = request.DataStore.GetValue("DatabricksClusterId");


            Thread.Sleep(Constants.ACTION_WAIT_INTERVAL);

            ClusterService client = new ClusterService(selectedLocation, azureTokenDatabricks);

            var cluster = await client.Get(databricksClusterId);

            if (cluster.State == Azure.Databricks.Model.ClusterState.RUNNING)
            {
                return new ActionResponse(ActionStatus.Success);
            }

            if (cluster.State == Azure.Databricks.Model.ClusterState.ERROR || cluster.State == Azure.Databricks.Model.ClusterState.TERMINATED)
            {
                return new ActionResponse(ActionStatus.Failure);
            }

            if (cluster.State != Azure.Databricks.Model.ClusterState.RUNNING)
            {
                return new ActionResponse(ActionStatus.InProgress);
            }

            return new ActionResponse(ActionStatus.InProgress);
        }
    }
}
