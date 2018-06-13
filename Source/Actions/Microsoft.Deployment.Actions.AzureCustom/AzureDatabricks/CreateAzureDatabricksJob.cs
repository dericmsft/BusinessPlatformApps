using Microsoft.Azure.Databricks.Jobs;
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
    public class CreateAzureDatabricksJob : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var selectedLocation = request.DataStore.GetJson("SelectedLocation", "Name");
            var azureTokenDatabricks = request.DataStore.GetValue("AzureTokenDatabricks");
            var notebookPath = request.DataStore.GetValue("NotebookPath");
            var clusterId = request.DataStore.GetValue("DatabricksClusterId");

            JobService service = new JobService(selectedLocation, azureTokenDatabricks);

            int jobId = await service.Create(clusterId, notebookPath);

            if (jobId > 0)
            {
                request.DataStore.AddToDataStore("DatabricksJobId", jobId);
                return new ActionResponse(ActionStatus.Success);
            }
            else
            {
                return new ActionResponse(ActionStatus.Failure);
            }
        }
    }
}
