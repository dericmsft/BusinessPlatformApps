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
    public class ADLSTests
    {
        [TestMethod]
        public async Task ADLAuth()
        {
            var dataStore = await TestManager.GetDataStore();
            var response = TestManager.ExecuteAction("Microsoft-GetDataLakeAccount", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}
