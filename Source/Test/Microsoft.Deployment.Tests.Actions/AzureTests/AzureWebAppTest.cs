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
    public class AzureWebAppTest
    {
        [TestMethod]
        public async Task CreateAzureWebApp()
        {
            var dataStore = await TestManager.GetDataStore();


			//// Deploy Function
			dataStore.AddToDataStore("DeploymentName", "WebApiDeploymentTest6");
			dataStore.AddToDataStore("webAppName", "KeyLinesTestApi6");
			dataStore.AddToDataStore("RepoUrl", "https://github.com/v-preben/Demo.git");
			dataStore.AddToDataStore("Branch", "master");
			dataStore.AddToDataStore("Project", "OnaWebApp/OnaWebApp.csproj");
			dataStore.AddToDataStore("ArmTemplateName", "AzureWebApp");
			dataStore.AddToDataStore("SettingName", "WebAppSettings");

			JObject obj = new JObject(new JProperty("hostName", "TestHostNameValue"),
										new JProperty("SPNKey", "TestSPNKeyValue"),
										new JProperty("SPNAppId", "TestSPNAppIdValue"),
										new JProperty("TenantId", "TestTenantId"),
										new JProperty("APUrl","TestValue"));

			dataStore.AddToDataStore("WebAppSettings", obj);


			var response = TestManager.ExecuteAction("Microsoft-DeployAzureWebApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
			response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
			Assert.IsTrue(response.IsSuccess);
			response = TestManager.ExecuteAction("Microsoft-DeployAzureWebAppApplicationSettings", dataStore);
			Assert.IsTrue(response.IsSuccess);
		}

        [TestMethod]
        public async Task CheckAppNameAvailability()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore("siteName", "KeyLinesTestSite99");
            var response = await TestManager.ExecuteActionAsync("Microsoft-CheckAppServiceNameAvailability", dataStore);
            Assert.IsFalse(response.IsSuccess);
        }
    }
}
