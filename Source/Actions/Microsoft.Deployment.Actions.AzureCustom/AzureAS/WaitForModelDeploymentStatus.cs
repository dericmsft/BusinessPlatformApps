using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AnalysisServices.Tabular;
using Microsoft.Deployment.Actions.AzureCustom.Wpa.Utilities;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureAS
{
    [Export(typeof(IAction))]
    public class WaitForModelDeploymentStatus : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureTokenAS");
            string serverUrl = request.DataStore.GetValue("ASServerUrl");
            string asDatabase = request.DataStore.GetValue("ASDatabase");

            //get data store location and connection string 
            //get get key for the blob storage 
            string connectionString = ValidateConnectionToAS.GetASConnectionString(request, azureToken, serverUrl);


            Server server = null;
            try
            {
                server = new Server();
                server.Connect(connectionString);

                for (; ; )
                {
                    Thread.Sleep(Constants.ACTION_WAIT_INTERVAL);
                    Database db = server.Databases.FindByName(asDatabase);
                    if (db != null)
                    {
                        var modelState = db.State;
                        if (modelState == AnalysisServices.AnalysisState.Processed)
                        {
                            server.Disconnect(true);
                            return new ActionResponse(ActionStatus.Success);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                request.Logger.LogException(e);
                return new ActionResponse(ActionStatus.Failure, string.Empty, e, "ErroDeployingModel");
            }
            finally
            {
                server?.Dispose();
            }

        }
    }
}