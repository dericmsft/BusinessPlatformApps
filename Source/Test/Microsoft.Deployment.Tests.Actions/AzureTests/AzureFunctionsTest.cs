using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Tests.Actions.AzureTests
{
    [TestClass]
    public class AzureFunctionsTest
    {
        [TestMethod]
        public async Task CreateAzureFunctionAndDeployConnectionString()
        {
            //var dataStore = await TestManager.GetDataStore();

            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            extraTokens.Add("as", "AzureTokenAS"); // request AAS token 

            // Deploy AS Model based of the following pramaters
            var dataStore = await TestManager.GetDataStore(true, extraTokens);

            dataStore.AddToDataStore("SelectedResourceGroup", "voloas");

            //// Deploy Function

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionName", "unittestfunction6");
            dataStore.AddToDataStore("RepoUrl", "https://github.com/dericmsft/AnalysisServicesRefresh");
            dataStore.AddToDataStore("sku", "Standard");
            

            var response = TestManager.ExecuteAction("Microsoft-DeployAzureFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //// Deploy Function
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("StorageAccountName", "exppltsa");
            dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
            dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "true");

            //var response = TestManager.ExecuteAction("Microsoft-CreateAzureStorageAccount", dataStore);
            //Assert.IsTrue(response.IsSuccess);
            //response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            //Assert.IsTrue(response.IsSuccess);

            response = TestManager.ExecuteAction("Microsoft-GetStorageAccountKey", dataStore);
            Assert.IsTrue(response.IsSuccess);

            JObject val = new JObject();
            val.Add("queue", dataStore.GetValue("StorageAccountConnectionString"));
            dataStore.AddToDataStore("AppSettingKeys", val);

            response = TestManager.ExecuteAction("Microsoft-DeployAzureFunctionAppSettings", dataStore);
            Assert.IsTrue(response.IsSuccess);


        }


        [TestMethod]
        public async Task CreateAzureFunctionForNews()
        {
            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            extraTokens.Add("as", "AzureTokenAS"); // request AAS token 

            // Deploy AS Model based of the following pramaters
            var dataStore = await TestManager.GetDataStore(true, extraTokens);

            dataStore.AddToDataStore("SelectedResourceGroup", "voloas");

            //// Deploy Function
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionName", "unittestfunction8");
            dataStore.AddToDataStore("RepoUrl", "https://github.com/dericmsft/AnalysisServicesRefresh");

            var response = TestManager.ExecuteAction("Microsoft-DeployAzureFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestManager.ExecuteAction("Microsoft-DeployPrivateAssemblyToFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeployFunctionAssetsNews()
        {
            var dataStore = await TestManager.GetDataStore();

            //// Deploy Function
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionName", "testfunctionappe");
            dataStore.AddToDataStore("apiKey", "655c7b05d8a04bf6a88647c54fd042e2");

            var response = TestManager.ExecuteAction("Microsoft-DeployNewsFunctionAsset", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task HistoricalLogicAppTest()
        {
            var dataStore = await TestManager.GetDataStore();

            //// Deploy Function
            dataStore.AddToDataStore("DeploymentName", "HistoricalAppTest");
            dataStore.AddToDataStore("LogicAppNameHistorical", "LogicAppNameHistorical");
            dataStore.AddToDataStore("SearchQuery", "Microsoft OR Azure");
            dataStore.AddToDataStore("FunctionName", "function1737e9shqv");

            var response = TestManager.ExecuteAction("Microsoft-DeployNewsTemplateHistorical", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestManager.ExecuteAction("Microsoft-RunNewsHistorical", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

    }
}
