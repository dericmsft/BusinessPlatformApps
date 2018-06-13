﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.Deployment.Common.ActionModel;

namespace Microsoft.Deployment.Tests.Actions.AzureTests
{
    /// <summary>
    /// Summary description for DeployAzureAnalysisServicesBlobStorageTestcs
    /// </summary>
    [TestClass]
    public class AzureAnalysisServicesBlobStorageTestcs
    {


        [TestMethod]
        public async Task CheckServerNameAvailability()
        {
            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            extraTokens.Add("as", "AzureTokenAS"); // request AAS token 
            // Deploy AS Model based of the following pramaters
            var dataStore = await TestManager.GetDataStore(true, extraTokens);

            dataStore.AddToDataStore("ASServerName", "wpaastest1");
            var response = await TestManager.ExecuteActionAsync("Microsoft-CheckASServerNameAvailability", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeployASModelTest()
        {
            try
            {

                Dictionary<string, string> extraTokens = new Dictionary<string, string>();
                extraTokens.Add("as", "AzureTokenAS"); // request AAS token 

                // Deploy AS Model based of the following pramaters
                var dataStore = await TestManager.GetDataStore(true, extraTokens);

                dataStore.AddToDataStore("StorageAccountName", "vmishel0604sa");
                dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
                dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "true");
                dataStore.AddToDataStore("SelectedResourceGroup", "vmishel0604rg");
                dataStore.AddToDataStore("StorageAccountContainer", "vmishel0604hdicl-2018-06-04t18-21-58-728z");
                dataStore.AddToDataStore("StorageAccountDirectory", "demobigdata");

                ActionResponse response = TestManager.ExecuteAction("Microsoft-GetStorageAccountKey", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("blobUrl", "https://vmishel0604sa.blob.core.windows.net/vmishel0604hdicl-2018-06-04t18-21-58-728z/Model/modelDefinition.json");
                dataStore.AddToDataStore("blobContentName", "asModelDefinition");

                response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("blobUrl", "https://vmishel0604sa.blob.core.windows.net/vmishel0604hdicl-2018-06-04t18-21-58-728z/Model/personHistoricalColumns.json");
                dataStore.AddToDataStore("blobContentName", "asPersonHistoricalDefinition");

                response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("ASServerName", "vfrortest2");
                dataStore.AddToDataStore("ASServerUrl", "asazure://westus.asazure.windows.net/vfrortest2");
                dataStore.AddToDataStore("ASLocation", "westus");
                dataStore.AddToDataStore("ASDatabase", "SemanticModel");

                dataStore.AddToDataStore("modelFilePath", "Service/AzureAS/modelDefinition.json");

                response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureASModelBlobStorage", dataStore, "Microsoft-WorkplaceAnalytics");
                Assert.IsTrue(response.IsSuccess);

                response = await TestManager.ExecuteActionAsync("Microsoft-WaitForModelDeploymentStatus", dataStore, "Microsoft-WorkplaceAnalytics");

                Assert.IsTrue(response.IsSuccess);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [TestMethod]
        public async Task DeployASModelTest2()
        {
            try
            {

                Dictionary<string, string> extraTokens = new Dictionary<string, string>();
                extraTokens.Add("as", "AzureTokenAS"); // request AAS token 

                // Deploy AS Model based of the following pramaters
                var dataStore = await TestManager.GetDataStore(true, extraTokens);

                dataStore.AddToDataStore("StorageAccountName", "exppltsa");
                dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
                dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "true");
                dataStore.AddToDataStore("SelectedResourceGroup", "voloas");
                dataStore.AddToDataStore("StorageAccountContainer", "rawdata");
                //dataStore.AddToDataStore("StorageAccountDirectory", "demobigdata");

                ActionResponse response = TestManager.ExecuteAction("Microsoft-GetStorageAccountKey", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("blobUrl", "https://exppltsa.blob.core.windows.net/rawdata/Model/modelDefinition.json");
                dataStore.AddToDataStore("blobContentName", "asModelDefinition");

                response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("blobUrl", "https://exppltsa.blob.core.windows.net/rawdata/Model/personHistoricalColumns.json");
                dataStore.AddToDataStore("blobContentName", "asPersonHistoricalDefinition");

                response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("ASServerName", "vfrortest2");
                dataStore.AddToDataStore("ASServerUrl", "asazure://westus.asazure.windows.net/vfrortest2");
                dataStore.AddToDataStore("ASLocation", "westus");
                dataStore.AddToDataStore("ASDatabase", "SemanticModel");

                dataStore.AddToDataStore("modelFilePath", "Service/AzureAS/modelDefinition.json");

                response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureASModelBlobStorage", dataStore, "Microsoft-WorkplaceAnalytics");
                Assert.IsTrue(response.IsSuccess);

                response = await TestManager.ExecuteActionAsync("Microsoft-WaitForModelDeploymentStatus", dataStore, "Microsoft-WorkplaceAnalytics");

                Assert.IsTrue(response.IsSuccess);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [TestMethod]
        public async Task DeployASModelOnFunction()
        {
            try
            {

                Dictionary<string, string> extraTokens = new Dictionary<string, string>();
                extraTokens.Add("as", "AzureTokenAS"); // request AAS token 

                // Deploy AS Model based of the following pramaters
                var dataStore = await TestManager.GetDataStore(true, extraTokens);

                dataStore.AddToDataStore("StorageAccountName", "exppltsa");
                dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
                dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "true");
                dataStore.AddToDataStore("SelectedResourceGroup", "voloas");
                dataStore.AddToDataStore("StorageAccountContainer", "rawdata");

                ActionResponse response = TestManager.ExecuteAction("Microsoft-GetStorageAccountKey", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("blobUrl", "https://exppltsa.blob.core.windows.net/rawdata/Model/modelDefinition.json");
                dataStore.AddToDataStore("blobContentName", "asModelDefinition");

                response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("blobUrl", "https://exppltsa.blob.core.windows.net/rawdata/Model/personHistoricalColumns.json");
                dataStore.AddToDataStore("blobContentName", "asPersonHistoricalDefinition");

                response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("ASServerName", "wpads");
                dataStore.AddToDataStore("ASServerUrl", "asazure://westus2.asazure.windows.net/wpads");
                dataStore.AddToDataStore("ASLocation", "westus2");
                dataStore.AddToDataStore("ASDatabase", "SemanticModel1");

                dataStore.AddToDataStore("modelFilePath", "Service/AzureAS/modelDefinition.json");

                response = await TestManager.ExecuteActionAsync("Microsoft-ExecuteAzureFunction", dataStore, "Microsoft-WorkplaceAnalytics");
                Assert.IsTrue(response.IsSuccess);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [TestMethod]
        public async Task CheckASConnection()
        {
            // Deploy AS Model based of the following pramaters
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore("ASServerUrl", "asazure://westus.asazure.windows.net/test");

            var response = await TestManager.ExecuteActionAsync("Microsoft-ValidateConnectionToAS", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task CheckPermissionCreation()
        {
            // Deploy AS Model based of the following pramaters
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore("ASServerUrl", "asazure://westus.asazure.windows.net/testmo");

            dataStore.AddToDataStore("SqlConnectionString", SqlCreds.GetSqlPagePayload("modb1"));
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Database/");
            dataStore.AddToDataStore("ASServerName", "testmo");
            dataStore.AddToDataStore("ASLocation", "westcentralus");
            dataStore.AddToDataStore("ASSku", "D1");
            dataStore.AddToDataStore("xmlaFilePath", "Service/AzureAS/SalesManagement.xmla");
            dataStore.AddToDataStore("ASDatabase", "testdb");
            dataStore.AddToDataStore("UserToAdd", "mohaali@microsoft.com");

            var response = await TestManager.ExecuteActionAsync("Microsoft-ValidateConnectionToAS", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureASModel", dataStore, "Microsoft-CRMSalesManagement");
            Assert.IsTrue(response.IsSuccess);

            response = await TestManager.ExecuteActionAsync("Microsoft-AssignPermissionsForUser", dataStore, "Microsoft-CRMSalesManagement");
            Assert.IsTrue(response.IsSuccess);
        }


        [TestMethod]
        public async Task GetJsonModel()
        {

            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            extraTokens.Add("as", "AzureTokenAS"); // request AAS token 

            var dataStore = await TestManager.GetDataStore(false, extraTokens);

            dataStore.AddToDataStore("ASServerUrl", "asazure://westus.asazure.windows.net/test");

            dataStore.AddToDataStore("StorageAccountName", "exppltsa");
            dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
            dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "true");

            dataStore.AddToDataStore("SelectedResourceGroup", "voloas");

            ActionResponse response = TestManager.ExecuteAction("Microsoft-GetStorageAccountKey", dataStore);

            dataStore.AddToDataStore("StorageAccountDirectory", "Model");
            dataStore.AddToDataStore("StorageAccountContainer", "rawdata");
            dataStore.AddToDataStore("blobUrl", "https://exppltsa.blob.core.windows.net/rawdata/Model/modelDefinition.json");
            dataStore.AddToDataStore("blobContentName", "asModelDefinition");

            response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

            Assert.IsTrue(response.IsSuccess);

            dataStore.AddToDataStore("blobUrl", "https://exppltsa.blob.core.windows.net/rawdata/Model/personHistoricalColumns.json");
            dataStore.AddToDataStore("blobContentName", "asPersonHistoricalDefinition");

            response = await TestManager.ExecuteActionAsync("Microsoft-GetASJsonBlob", dataStore);

            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task WaitForModelTest()
        {
            try
            {

                Dictionary<string, string> extraTokens = new Dictionary<string, string>();
                extraTokens.Add("as", "AzureTokenAS"); // request AAS token 

                // Deploy AS Model based of the following pramaters
                var dataStore = await TestManager.GetDataStore(true, extraTokens);

                dataStore.AddToDataStore("StorageAccountName", "exppltsa");
                dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
                dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "true");
                dataStore.AddToDataStore("SelectedResourceGroup", "voloas");
                dataStore.AddToDataStore("StorageAccountContainer", "rawdata");

                ActionResponse response = TestManager.ExecuteAction("Microsoft-GetStorageAccountKey", dataStore);

                Assert.IsTrue(response.IsSuccess);

                dataStore.AddToDataStore("ASServerName", "vfrortest20");
                dataStore.AddToDataStore("ASServerUrl", "asazure://westus.asazure.windows.net/vfrortest20");
                dataStore.AddToDataStore("ASLocation", "westus");
                dataStore.AddToDataStore("ASDatabase", "SemanticModel");

                dataStore.AddToDataStore("modelFilePath", "Service/AzureAS/modelDefinition.json");

                response = await TestManager.ExecuteActionAsync("Microsoft-WaitForModelDeploymentStatus", dataStore, "Microsoft-WorkplaceAnalytics");
                Assert.IsTrue(response.IsSuccess);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

    }
}
