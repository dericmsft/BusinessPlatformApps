using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataLake.Store;
using Microsoft.Azure.Management.DataLake.Store.Models;
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

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string aadTenant = request.DataStore.GetValue("AADTenant");
            var deploymentName = request.DataStore.GetValue("DeploymentName") ?? "ADLSDeployment";
            var dataLakeName = "adls42";
            DataLakeStoreAccount account = null;

            var creds = GetCreds(Constants.MSFTTenant, new Uri(Constants.AzureManagementCoreApi), Constants.MicrosoftADLClientId, Constants.MicrosoftADLClientSecret);

            client = new DataLakeStoreAccountManagementClient(creds);
            client.SubscriptionId = subscription;

            try
            {
                account = client.Account.Get(resourceGroup, dataLakeName);
            }
            catch(Exception ex)
            {
                if(!ex.Message.ToLower().Contains("not found"))
                {
                    throw ex;
                }
            }

            if (account == null)
            {
                account = client.Account.CreateAsync(resourceGroup, dataLakeName, new Azure.Management.DataLake.Store.Models.DataLakeStoreAccount("centralus")).Result;
            }

            return new ActionResponse(ActionStatus.Success, account.Endpoint);
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
