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
    public class RunAzureDatabricksJob : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var selectedLocation = request.DataStore.GetJson("SelectedLocation", "Name");
            var azureTokenDatabricks = request.DataStore.GetValue("AzureTokenDatabricks");
            var notebookPath = request.DataStore.GetValue("NotebookPath");
            string str_jobId = request.DataStore.GetValue("DatabricksJobId");

            JobService service = new JobService(selectedLocation, azureTokenDatabricks);

            int jobId = Int32.Parse(str_jobId);

            var runId = await service.Run(jobId);

            if (runId > 0)
            {
                request.DataStore.AddToDataStore("DatabricksRunId", runId);
                return new ActionResponse(ActionStatus.Success);
            }
            else
            {
                return new ActionResponse(ActionStatus.Failure);
            }
        }
    }
}
