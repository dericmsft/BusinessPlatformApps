using System.ComponentModel.Composition;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.WebApp
{
    [Export(typeof(IAction))]
    public class CheckAppServiceNameAvailability : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureTokenAS", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string name = request.DataStore.GetValue("siteName");
            string location = request.DataStore.GetValue("ASLocation") ?? "westus"; 

            // checking the app service hosting plan
            AzureHttpClient clientService = new AzureHttpClient(azureToken, subscription);
            HttpResponseMessage responseService = await clientService.ExecuteWithSubscriptionAsync(
                HttpMethod.Get,
                $"resourceGroups/{resourceGroup}/providers/Microsoft.Web/serverfarms/{name}",
                "2016-09-01","");

            // checking the app service 
            AzureHttpClient clientApp = new AzureHttpClient(azureToken, subscription);
            HttpResponseMessage responseApp = await clientApp.ExecuteWithSubscriptionAsync(
                HttpMethod.Get,
                $"resourceGroups/{resourceGroup}/providers/Microsoft.Web/sites/{name}",
                "2016-08-01", "");

            if (responseService.IsSuccessStatusCode || responseApp.IsSuccessStatusCode)
            {
                string body = await responseService.Content.ReadAsStringAsync();
                var json = JsonUtility.GetJsonObjectFromJsonString(body);
                if (json["name"].ToString().ToLower() == name.ToLower())
                {
                    return new ActionResponse(ActionStatus.FailureExpected, json, null, null, "A web app or app service plan exists with the same name");
                }
            }
            else if (responseService.StatusCode == System.Net.HttpStatusCode.NotFound && responseApp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ActionResponse(ActionStatus.Success);
            }

            return new ActionResponse(ActionStatus.Failure, null, null, null, "Unable to query Azure for name availability");
        }
    }
}