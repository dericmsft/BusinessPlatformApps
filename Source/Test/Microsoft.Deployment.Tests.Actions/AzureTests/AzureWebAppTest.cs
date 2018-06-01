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
    public class AzureWebAppTest
    {
        [TestMethod]
        public async Task CreateAzureWebApp()
        {
            var dataStore = await TestManager.GetDataStore();

            //// Deploy Function
            dataStore.AddToDataStore("DeploymentName", "WebAppDeploymentTest");
            dataStore.AddToDataStore("siteName", "KeyLinesSite7");
            dataStore.AddToDataStore("RepoUrl", "https://github.com/v-preben/Demo.git");
            //dataStore.AddToDataStore("HostingPlanName", "KeyLinesSite7");

            var response = TestManager.ExecuteAction("Microsoft-DeployAzureWebApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
           
        }
    }
}
