﻿using System;
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
            dataStore.AddToDataStore("DeploymentName", "WebAppDeploymentTest3");
            dataStore.AddToDataStore("siteName", "KeyLinesTestSite5");
            dataStore.AddToDataStore("RepoUrl", "https://github.com/v-preben/Demo.git");
            dataStore.AddToDataStore("Branch", "Test");
            dataStore.AddToDataStore("Project", "KeyLinesDemo2/KeyLinesDemo2.csproj");
            dataStore.AddToDataStore("ArmTemplate", "AzureWebApp");
            dataStore.AddToDataStore("IsApi", "false"); // code can be removed if this is included in the app settings script
            dataStore.AddToDataStore("HostName", "keylinestestapi1.azurewebsites.net");

            var response = TestManager.ExecuteAction("Microsoft-DeployAzureWebApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
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
