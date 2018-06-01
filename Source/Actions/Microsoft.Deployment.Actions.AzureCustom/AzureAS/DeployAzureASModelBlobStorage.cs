using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AnalysisServices.Tabular;
using Microsoft.Deployment.Actions.AzureCustom.Wpa.Utilities;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureAS
{
    [Export(typeof(IAction))]
    public class DeployAzureASModelBlobStorage : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureTokenAS");
            string serverUrl = request.DataStore.GetValue("ASServerUrl");
            string asDatabase = request.DataStore.GetValue("ASDatabase");
            string modelFile = request.DataStore.GetValue("modelFilePath");
            string modelDefinition = request.DataStore.GetValue("asModelDefinition");
            string personHistorialDefinition = request.DataStore.GetValue("asPersonHistoricalDefinition");
            string storageAccountName = request.DataStore.GetValue("StorageAccountName");
            string storageAccountKey = request.DataStore.GetValue("StorageAccountKey");
            string containerName = request.DataStore.GetValue("StorageAccountContainer");
            string folderName = request.DataStore.GetValue("StorageAccountDirectory");
            //get data store location and connection string 
            //get get key for the blob storage 
            try
            {
                string connectionString = ValidateConnectionToAS.GetASConnectionString(request, azureToken, serverUrl);


                string jsonContents = !string.IsNullOrWhiteSpace(modelDefinition) ? modelDefinition : File.ReadAllText(request.Info.App.AppFilePath + "/" + modelFile);

                if (!string.IsNullOrWhiteSpace(personHistorialDefinition))
                {
                    jsonContents = ModelParser.AddColumns(jsonContents, "PersonHistorical", personHistorialDefinition);
                }

                ModelParser parser = new ModelParser(jsonContents, storageAccountName, storageAccountKey, containerName, folderName);

                string jsonAsDatabaseDefinition = parser.Parse();

                Task.Run(() => DeployAASModel(jsonAsDatabaseDefinition, connectionString, asDatabase));

                return new ActionResponse(ActionStatus.Success);
            }
            catch (Exception e)
            {
                request.Logger.LogException(e);
                return new ActionResponse(ActionStatus.Failure, string.Empty, e, "ErroDeployingModel");
            }

        }

        public async void DeployAASModel(string jsonModel, string connectionString, string asDatabase)
        {
            Server server = null;
            try
            {
                server = new Server();
                server.Connect(connectionString);

                // Delete existing
                Database db = server.Databases.FindByName(asDatabase);
                db?.Drop();

                db = JsonSerializer.DeserializeDatabase(jsonModel);

                server.Databases.Add(db);

                db.Model.RequestRefresh(AnalysisServices.Tabular.RefreshType.Full);
                db.Update(Microsoft.AnalysisServices.UpdateOptions.ExpandFull);

                
                server.Disconnect(true);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                server?.Dispose();
            }
        }
    }
}