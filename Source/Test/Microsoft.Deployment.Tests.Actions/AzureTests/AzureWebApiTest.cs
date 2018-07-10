using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft;
using Newtonsoft.Json.Linq;

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
            dataStore.AddToDataStore("DeploymentName", "WebApiDeploymentTest6");
            dataStore.AddToDataStore("webAppName", "KeyLinesTestApi6");
            dataStore.AddToDataStore("RepoUrl", "https://github.com/v-preben/Demo.git");
            dataStore.AddToDataStore("Branch", "master");
            dataStore.AddToDataStore("Project", "WpASolutions.AnalyticsServiceAdapter/WpASolutions.AnalyticsServiceAdapter.csproj");
            dataStore.AddToDataStore("ArmTemplateName", "AzureWebApi");
			dataStore.AddToDataStore("SettingName", "ApiAppSettings");

			JObject obj = new JObject(
										new JProperty("Server", "wpads"),
										new JProperty("AppId", "f40014d1-a331-4710-9662-3c92a4345e7a"),
										new JProperty("AppKey", "SNaW5kGqKrieBpSN5s2pUHcSiq+7duhS411SP13oqsQ="),
										new JProperty("Model", "expplt01"),
										new JProperty("AzureAd:TenantId", "72f988bf-86f1-41af-91ab-2d7cd011db47"));

			dataStore.AddToDataStore("ApiAppSettings", obj);


			//"{\"hostName\": \"test.azurewebsites.net\", " +
			//	"\"SPNKey\": \"SPNKey\",	" +
			//	"\"SPNAppId\": \"TestSPNAppId\",	" +
			//	"\"APUrl\": \"TestAPUrl\", " +
			//	"\"TenantId\": \"TestTenantId\"}");

			var response = TestManager.ExecuteAction("Microsoft-DeployAzureWebApp", dataStore);
			Assert.IsTrue(response.IsSuccess);
			response = TestManager.ExecuteAction("Microsoft-WaitForWpaArmDeploymentStatus", dataStore);
			Assert.IsTrue(response.IsSuccess);
			response = TestManager.ExecuteAction("Microsoft-DeployAzureWebAppApplicationSettings", dataStore);
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
