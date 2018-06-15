using Microsoft.Azure.Databricks.Clusters;
using Microsoft.Azure.Databricks.Model;
using Microsoft.Azure.Databricks.Tokens;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Tests.Actions.AzureTests
{
    [TestClass]
    public class DatabricksTests
    {
        [TestMethod]
        public async Task DeployDatabricksWorkspaceViaArmTemplate()
        {
            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            //extraTokens.Add("databricks", "DatabricksToken"); // r`equest AAS token 

            var dataStore = await TestManager.GetDataStore(true, extraTokens);

            JObject parameters = new JObject();
            parameters.Add("DatabricksWorkspaceName", "vfrortest-databricks");
            parameters.Add("location", "westus");
            parameters.Add("tier", "standard");
            dataStore.AddToDataStore("SelectedResourceGroup", "vfrordatabrickstest5-rg");
            dataStore.AddToDataStore("DeploymentName", "AzureDatabricks");
            dataStore.AddToDataStore("AzureArmFile", "Service/ARM/AzureDatabricks.json");
            dataStore.AddToDataStore("AzureArmParameters", parameters);
            var response = await TestManager.ExecuteActionAsync("Microsoft-DeplyAzureDatabricksWorkspace", dataStore, "Microsoft-WorkplaceAnalytics");
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeployCluster()
        {
            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            //extraTokens.Add("databricks", "DatabricksToken"); // r`equest AAS token 

            var dataStore = await TestManager.GetDataStore(true, extraTokens);

            dataStore.AddToDataStore("StorageAccountName", "vmishel0604sa");
            dataStore.AddToDataStore("AzureTokenDatabricks", "dapif8cdb7625853faa8b5eccc505f64d23f");
            //dataStore.AddToDataStore("SelectedLocation", "{Name: westus2}", DataStoreType.Public);
            dataStore.AddToDataStore("SelectedResourceGroup", "vmishel0604rg");
            dataStore.AddToDataStore("StorageAccountContainer", "demobigdata");
            
            dataStore.AddToDataStore("DatabricksClusterName", "FranciscoTestCluser1");
            //var databricksClusterName = request.DataStore.GetValue("DatabricksClusterName");

            ActionResponse response = TestManager.ExecuteAction("Microsoft-GetStorageAccountKey", dataStore);

            Assert.IsTrue(response.IsSuccess);

            //response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureDatabricksCluster", dataStore);

            Assert.IsTrue(response.IsSuccess);

            //dataStore.AddToDataStore("DatabricksClusterId", "0613-010942-femur361");

            //while (true)
            //{
            //    response = await TestManager.ExecuteActionAsync("Microsoft-WaitForClusterDeploymentStatus", dataStore);
            //    if (response.Status == ActionStatus.Success)
            //    {
            //        break;
            //    }
            //}
            var azureTokenDatabricks = dataStore.GetValue("AzureTokenDatabricks");
            TokenService s = new TokenService("westus", azureTokenDatabricks);

            List<Token> t = await s.List();

            Assert.IsTrue(response.IsSuccess);

            dataStore.AddToDataStore("blobUrl", "https://vmishel0604sa.blob.core.windows.net/demobigdata/Model/CreateInteractionsTest.py");
            dataStore.AddToDataStore("blobContentName", "NotebookContent");

            response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

            response = await TestManager.ExecuteActionAsync("Microsoft-ImportAzureDatabricksNotebook", dataStore);

            Assert.IsTrue(response.IsSuccess);

            //dataStore.AddToDataStore("NotebookPath", "/Shared/InteractionsNotebook_API_Test1");

            response = await TestManager.ExecuteActionAsync("Microsoft-CreateAzureDatabricksJob", dataStore);

            Assert.IsTrue(response.IsSuccess);

            response = await TestManager.ExecuteActionAsync("Microsoft-RunAzureDatabricksJob", dataStore);

            Assert.IsTrue(response.IsSuccess);

            //dataStore.AddToDataStore("DatabricksRunId", "25");

            while(true)
            {
                response = await TestManager.ExecuteActionAsync("Microsoft-WaitForAzureDatabricksRunStatus", dataStore);
                if(response.Status == ActionStatus.Success)
                {
                    break;
                }
            }            

            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task CheckDatabricksWorkspaceAvailabilityTest()
        {
            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            //extraTokens.Add("databricks", "DatabricksToken"); // r`equest AAS token 

            var dataStore = await TestManager.GetDataStore(true, extraTokens);

            JObject parameters = new JObject();
            parameters.Add("DatabricksWorkspaceName", "vfrortest-databricks");
            parameters.Add("location", "westus");
            parameters.Add("tier", "standard");
            dataStore.AddToDataStore("DatabricksWorkspaceName", "vfrortest-databricks");
            dataStore.AddToDataStore("SelectedResourceGroup", "voloas");
            dataStore.AddToDataStore("DeploymentName", "AzureDatabricks");
            dataStore.AddToDataStore("AzureArmFile", "Service/ARM/AzureDatabricks.json");
            dataStore.AddToDataStore("AzureArmParameters", parameters);
            var response = await TestManager.ExecuteActionAsync("Microsoft-CheckDatabricksWorkspaceAvailability", dataStore, "Microsoft-WorkplaceAnalytics");
            Assert.IsTrue(response.IsSuccess);
        }
        [TestMethod]
        public async Task CheckDatabricksWorkspaceAvailabilityNotAvailableTest()
        {
            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            //extraTokens.Add("databricks", "DatabricksToken"); // r`equest AAS token 

            var dataStore = await TestManager.GetDataStore(true, extraTokens);

            JObject parameters = new JObject();
            parameters.Add("DatabricksWorkspaceName", "vfrordatabrickstest5");
            parameters.Add("location", "westus");
            parameters.Add("tier", "standard");
            dataStore.AddToDataStore("DatabricksWorkspaceName", "vfrordatabrickstest5");
            dataStore.AddToDataStore("SelectedResourceGroup", "vfrordatabrickstest5-rg");
            dataStore.AddToDataStore("DeploymentName", "AzureDatabricks");
            dataStore.AddToDataStore("AzureArmFile", "Service/ARM/AzureDatabricks.json");
            dataStore.AddToDataStore("AzureArmParameters", parameters);
            var response = await TestManager.ExecuteActionAsync("Microsoft-CheckDatabricksWorkspaceAvailability", dataStore, "Microsoft-WorkplaceAnalytics");
            Assert.IsTrue(response.IsSuccess);
        }


        [TestMethod]
        public async Task GetWorkspace()
        {
            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            //extraTokens.Add("databricks", "DatabricksToken"); // r`equest AAS token 

            var dataStore = await TestManager.GetDataStore(true, extraTokens);

            JObject parameters = new JObject();
            parameters.Add("DatabricksWorkspaceName", "vfrordatabrickstest5");
            parameters.Add("location", "westus");
            parameters.Add("tier", "standard");
            dataStore.AddToDataStore("DatabricksWorkspaceName", "vfrordatabrickstest5");
            dataStore.AddToDataStore("SelectedResourceGroup", "vfrordatabrickstest5-rg");
            dataStore.AddToDataStore("DeploymentName", "AzureDatabricks");
            dataStore.AddToDataStore("AzureArmFile", "Service/ARM/AzureDatabricks.json");
            dataStore.AddToDataStore("AzureArmParameters", parameters);

            string azureToken = dataStore.GetJson("AzureToken", "access_token");
            string subscription = dataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = dataStore.GetValue("SelectedResourceGroup");
            var workspaceName = "vfrordatabrickstest5";

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);
            var response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Get
                , $"/providers/Microsoft.Databricks/workspaces/{workspaceName}/"
                , "2018-04-01"
                , string.Empty
                , new Dictionary<string, string>());

            string responseBody = await response.Content.ReadAsStringAsync();

            JObject responseObj = JsonUtility.GetJObjectFromJsonString(responseBody);
            dataStore.AddToDataStore("WorkspaceId", responseObj["id"], DataStoreType.Public);
            dataStore.AddToDataStore("WorkspaceLocation", responseObj["location"], DataStoreType.Public);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine(responseBody);
            }
        }


        
    }
}
