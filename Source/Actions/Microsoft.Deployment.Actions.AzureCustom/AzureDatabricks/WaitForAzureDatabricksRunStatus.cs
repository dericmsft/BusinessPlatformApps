using Microsoft.Azure.Databricks.Jobs;
using Microsoft.Azure.Databricks.Model;
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
    public class WaitForAzureDatabricksRunStatus : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var selectedLocation = request.DataStore.GetJson("SelectedLocation", "Name");
            var azureTokenDatabricks = request.DataStore.GetValue("AzureTokenDatabricks");
            var notebookPath = request.DataStore.GetValue("NotebookPath");
            string str_runId = request.DataStore.GetValue("DatabricksRunId");

            Thread.Sleep(Constants.ACTION_WAIT_INTERVAL);

            JobService service = new JobService(selectedLocation, azureTokenDatabricks);

            int runId = Int32.Parse(str_runId);

            RunState state = await service.GetRunState(runId);

            if(state.LifeCycleState == RunLifeCycleState.INTERNAL_ERROR)
            {
                return new ActionResponse(ActionStatus.Failure, "Job Service Failue");
            }
            if (state.LifeCycleState == RunLifeCycleState.TERMINATED)
            {
                if(state.ResultState == RunResultState.SUCCESS)
                {
                    return new ActionResponse(ActionStatus.Success);
                }
                else
                {
                    return new ActionResponse(ActionStatus.Failure);
                }
            }
            else
            {
                return new ActionResponse(ActionStatus.InProgress);
            }
        }
    }
}
