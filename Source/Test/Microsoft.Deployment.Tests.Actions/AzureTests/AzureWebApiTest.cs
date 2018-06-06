using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.AzureTests
{
    [TestClass]
    public class AzureWebApiTest
    {
        [TestMethod]
        public async Task CreateAzureWebApi()
        {
            var dataStore = await TestManager.GetDataStore();

            //// Deploy Function
            dataStore.AddToDataStore("DeploymentName", "WebApiDeploymentTest1");
            dataStore.AddToDataStore("siteName", "KeyLinesTestApi1");
            dataStore.AddToDataStore("RepoUrl", "https://github.com/v-preben/Demo.git");
            dataStore.AddToDataStore("Branch", "Test");
            dataStore.AddToDataStore("Project", "WpASolutions.AnalyticsServiceAdapter/WpASolutions.AnalyticsServiceAdapter.csproj");
            dataStore.AddToDataStore("ArmTemplate", "AzureWebApi");
            dataStore.AddToDataStore("IsApi", "true"); // code can be removed if this is included in the app settings script

            var response = TestManager.ExecuteAction("Microsoft-DeployAzureWebApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }


        [TestMethod]
        public async Task CheckApiNameAvailability()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore("siteName", "keylinestestapi3");
            var response = await TestManager.ExecuteActionAsync("Microsoft-CheckAppServiceNameAvailability", dataStore);
            Assert.IsFalse(response.IsSuccess);
        }
    }
}
