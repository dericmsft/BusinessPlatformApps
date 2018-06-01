using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Azure;

namespace Microsoft.Deployment.Actions.AzureCustom.Functions
{
    [Export(typeof(IAction))]
    public class ExecuteAzureFunction : BaseAction
    {
        public const int ATTEMPTS = 92;
        public const int STATUS = 4;
        public const int WAIT = 5000;

        public const string SUCCESS = "Deployment successful";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var sitename = request.DataStore.GetValue("FunctionName");


            var apiURL = "https://asschedulerab7f6v4w7508.azurewebsites.net/api/ASDeployModel?code=kb1g4navgsewGeI4JJRbpswy5/0tS0GjwxEsVQQtRlRl4ukbD432Dw==";
            HttpClient client = new HttpClient();
            HttpRequestMessage functionRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(apiURL));
            var body = "{}";
            functionRequest.Content = new StringContent( body, System.Text.Encoding.UTF8, "application/json");
            functionRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync(apiURL, functionRequest.Content);

            var result = await response.Content.ReadAsStringAsync();


            return response.IsSuccessStatusCode ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);
        }
    }
}
