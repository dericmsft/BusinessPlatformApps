//using System;
//using System.ComponentModel.Composition;
//using System.Dynamic;
//using System.Net.Http;
//using System.Security.Cryptography;
//using System.Threading.Tasks;

//using Newtonsoft.Json.Linq;

//using Microsoft.Deployment.Common.ActionModel;
//using Microsoft.Deployment.Common.Actions;
//using Microsoft.Deployment.Common.Helpers;

//namespace Microsoft.Deployment.Actions.AzureCustom.WebApp
//{
//	[Export(typeof(IAction))]
//	public class CreateAppSpn : BaseAction
//	{
//		public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
//		{
//			var azureToken = request.DataStore.GetJson("AzureTokenAS");
//			var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");

//			JObject graphToken = AzureTokenUtility.GetTokenForResourceFromExistingToken("as", request.Info.WebsiteRootUrl, azureToken, "https://graph.windows.net");
//			var tenantId = AzureUtility.GetTenantFromToken(request.DataStore.GetJson("AzureToken"));

//			// Generate new key for ClientSecret
//			string key = GetNewKey();
//			string graphUriBase = "https://graph.windows.net/{0}/applications";

//			string graphApi = string.Format(graphUriBase, tenantId);

//			AzureHttpClient client = new AzureHttpClient(graphToken["access_token"].ToString(), subscription);
//			dynamic payload = new ExpandoObject();

//			payload.displayName = "solutiontemplate";
//			payload.availableToOtherTenants = false;
//			payload.homepage = "www.test.com";
//			payload.identifierUris = new string[1];
//			payload.identifierUris[0] = "https://test.com/" + RandomGenerator.GetRandomLowerCaseCharacters(10);

//			payload.passwordCredentials = new ExpandoObject[1];
//			payload.passwordCredentials[0] = new ExpandoObject();
//			payload.passwordCredentials[0].startDate = DateTime.UtcNow.ToString("o");
//			payload.passwordCredentials[0].endDate = DateTime.UtcNow.AddYears(3).ToString("o");
//			payload.passwordCredentials[0].keyId = Guid.NewGuid();
//			payload.passwordCredentials[0].value = key;

//			string body = JsonUtility.GetJsonStringFromObject(payload);

//			var response = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, graphApi + "?api-version=1.6", body);
//			string responseBody = await response.Content.ReadAsStringAsync();
//			JObject responseBodyObj = JsonUtility.GetJObjectFromJsonString(responseBody);
//			if (response.IsSuccessStatusCode)
//			{
//				string appId = responseBodyObj["appId"].ToString();
//				string obbId = responseBodyObj["objectId"].ToString();

//				responseBodyObj.Add("SPNAppId", appId);
//				responseBodyObj.Add("SPNKey", key);
//				responseBodyObj.Add("SPNUser", "app:" + appId + "@" + tenantId);
//				responseBodyObj.Add("SPNTenantId", tenantId);

//				// Delete the SPN if required 
//				//string graphUriBaseWithApplication = "https://graph.windows.net/{0}/applications/{1}";
//				//string graphApiWithApp = string.Format(graphUriBaseWithApplication, tenantId, obbId);
//				//response = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Delete, graphApiWithApp + "?api-version=1.6", body);

//				return new ActionResponse(ActionStatus.Success, responseBodyObj, true);
//			}

//			return new ActionResponse(ActionStatus.Failure, responseBody, null, null, "Unable to create a Service Principal");
//		}

//		private static string GetNewKey()
//		{
//			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
//			byte[] buffer = new byte[44];
//			rng.GetBytes(buffer);

//			// Return a Base64 string representation of the random number.
//			var key = Convert.ToBase64String(buffer);
//			return key;
//		}
//	}
//}