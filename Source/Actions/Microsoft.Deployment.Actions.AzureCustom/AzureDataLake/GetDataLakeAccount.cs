using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataLake.Store;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureDataLake
{
    [Export(typeof(IAction))]
    public class GetDataLakeAccount : BaseAction
    {
        private DataLakeStoreAccountManagementClient client;

        public override Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string aadTenant = request.DataStore.GetValue("AADTenant");
            var deploymentName = request.DataStore.GetValue("DeploymentName") ?? "ADLSDeployment";

            var creds = GetCreds(aadTenant, new Uri("https://datalake.azure.net/"), Constants.MicrosoftADLClientId, Constants.MicrosoftADLClientSecret);

            return null;

        }

        private ServiceClientCredentials GetCreds(string tenant, Uri tokenAudience, string clientId, string secretKey)
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var serviceSettings = ActiveDirectoryServiceSettings.Azure;
            serviceSettings.TokenAudience = tokenAudience;

            var creds = ApplicationTokenProvider.LoginSilentAsync(
             tenant,
             clientId,
             secretKey,
             serviceSettings).GetAwaiter().GetResult();
            return creds;
        }
    }
}
