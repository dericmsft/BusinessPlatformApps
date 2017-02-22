﻿using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class RegisterBapiService : BaseAction
    {
        private string BASE_AZURE_ENROLL_URL = "https://management.azure.com/providers/";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            string bapiService = request.DataStore.GetLastValue("BapiService");
            string subscriptionId = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();

            AzureHttpClient client = new AzureHttpClient(azureToken);

            string enrollBody = "{}";
            string enrollUrl = $"{BASE_AZURE_ENROLL_URL}/{bapiService}/enroll?api-version=2016-11-01&id=@id";

            var enrollResponse = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, enrollUrl, enrollBody);
            if (!(enrollResponse.StatusCode == System.Net.HttpStatusCode.OK || enrollResponse.StatusCode == System.Net.HttpStatusCode.Accepted))
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "RegisterProviderError");

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}