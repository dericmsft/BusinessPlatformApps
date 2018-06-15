using Microsoft.Azure.Databricks.Workspace;
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
    public class CheckDatabricksWorkspaceExists : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var selectedLocation = request.DataStore.GetJson("SelectedLocation", "Name");
            var azureTokenDatabricks = request.DataStore.GetValue("AzureTokenDatabricks");

            WorkspaceService service = new WorkspaceService(selectedLocation, azureTokenDatabricks);

            bool exists = await service.WorkspaceExists();

            if (exists)
            {
                return new ActionResponse(ActionStatus.Success);
            }
            else
            {
                return new ActionResponse(ActionStatus.Failure);
            }

        }
    }
}
