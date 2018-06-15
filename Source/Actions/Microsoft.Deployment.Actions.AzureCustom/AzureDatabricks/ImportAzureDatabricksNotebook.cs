using Microsoft.Azure.Databricks.Clusters;
using Microsoft.Azure.Databricks.Model;
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
    class ImportAzureDatabricksNotebook : BaseAction
    {

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {

            var selectedLocation = request.DataStore.GetJson("SelectedLocation", "Name");
            var azureTokenDatabricks = request.DataStore.GetValue("AzureTokenDatabricks");
            //var azureTokenDatabricks = request.DataStore.GetJson("AzureToken", "access_token");
            string notebookPath = request.DataStore.GetValue("NotebookPath");

            WorkspaceService service = new WorkspaceService(selectedLocation, azureTokenDatabricks);

            NotebookImport notebookImport = new NotebookImport();
            notebookImport.Path = "/Shared/InteractionsNotebook_API_Test1";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(request.DataStore.GetValue("NotebookContent"));
            notebookImport.Content = System.Convert.ToBase64String(plainTextBytes);
            notebookImport.Format = ExportFormat.SOURCE;
            notebookImport.Language = "PYTHON";

            bool imported = await service.NotebookImport(notebookImport);

            if (imported)
            {
                request.DataStore.AddToDataStore("NotebookPath", notebookImport.Path);
                return new ActionResponse(ActionStatus.Success);
            }
            else
            {
                return new ActionResponse(ActionStatus.Failure);
            }
        }
    }
}
