using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureAS
{
    [Export(typeof(IAction))]
    public class DeployAzureASModel : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureTokenAS");
            string serverUrl = request.DataStore.GetValue("ASServerUrl");

            string connectionType = request.DataStore.GetValue("ConnectionType");
            string xmla = request.DataStore.GetValue("xmlaFilePath");
            string asDatabase = request.DataStore.GetValue("ASDatabase");
            
            string connectionString = ValidateConnectionToAS.GetASConnectionString(request, azureToken, serverUrl);
            string xmlaContents = File.ReadAllText(request.Info.App.AppFilePath + "/" + xmla);
            Server server = null;

            if (connectionType == "azure-data-lake")
            {
                string dataLakeConnection = string.Concat("https://",request.DataStore.GetValue("DataLakeName"), ".azuredatalakestore.net");
                var token = request.DataStore.GetJson("AzureToken");
                var obj = JsonUtility.GetJsonObjectFromJsonString(xmlaContents);
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["connectionDetails"]["address"]["url"] = dataLakeConnection;
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["path"] = dataLakeConnection + "/";
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["AccessToken"] = token["access_token"];
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["RefreshToken"] = token["refresh_token"];
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["token_type"] = token["token_type"];
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["scope"] = token["scope"];
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["ext_expires_in"] = token["ext_expires_in"];
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["expires_on"] = token["expires_on"];
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["not_before"] = token["not_before"];
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["resource"] = token["resource"];
                obj["createOrReplace"]["database"]["model"]["dataSources"][0]["credential"]["id_token"] = token["id_token"];

                server = new Server();
                server.Connect(connectionString);
                XmlaResultCollection response = server.Execute(obj.ToString());

                if (response.ContainsErrors)
                {
                    return new ActionResponse(ActionStatus.Failure, response[0].Value);
                }

                server.Refresh(true);
                var db = server.Databases.FindByName(asDatabase);
                db.Update(UpdateOptions.ExpandFull);
                db.Model.RequestRefresh(AnalysisServices.Tabular.RefreshType.Full);
                db.Model.SaveChanges();

                return new ActionResponse(ActionStatus.Success);
            }
                        
            string sqlConnectionString = request.DataStore.GetValue("SqlConnectionString");
            var connectionStringObj = SqlUtility.GetSqlCredentialsFromConnectionString(sqlConnectionString);

            try
            {
                server = new Server();
                server.Connect(connectionString);

                // Delete existing
                Database db = server.Databases.FindByName(asDatabase);
                db?.Drop();

                // Deploy database definition
                var obj = JsonUtility.GetJsonObjectFromJsonString(xmlaContents);
                obj["create"]["database"]["name"] = asDatabase;
                XmlaResultCollection response = server.Execute(obj.ToString());

                if (response.ContainsErrors)
                {
                    return new ActionResponse(ActionStatus.Failure, response[0].Value);
                }

                // Reload metadata and update connection string
                server.Refresh(true);
                db = server.Databases.FindByName(asDatabase);
                ((ProviderDataSource)db.Model.DataSources[0]).ConnectionString = $"Provider=SQLNCLI11;Data Source=tcp:{connectionStringObj.Server};Persist Security Info=True;User ID={connectionStringObj.Username};Password={connectionStringObj.Password};Initial Catalog={connectionStringObj.Database}";
                db.Update(UpdateOptions.ExpandFull);

                // Process if there's a tag requesting it
                if (db.Model.DataSources[0].Annotations.ContainsName("MustProcess"))
                {
                    db.Model.RequestRefresh(AnalysisServices.Tabular.RefreshType.Full);
                    db.Model.SaveChanges();
                }

                server.Disconnect(true);

                return new ActionResponse(ActionStatus.Success);
            }
            catch (Exception e)
            {
                return new ActionResponse(ActionStatus.Failure, string.Empty, e, null);
            }
            finally
            {
                server?.Dispose();
            }

        }
    }
}