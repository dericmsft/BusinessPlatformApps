using Microsoft.Azure.Databricks.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Databricks.Model;

namespace Microsoft.Azure.Databricks.Tokens
{
    public class TokenService
    {
        string token;
        HttpClient client;

        public TokenService(string location, string token)
        {
            this.token = token;

            HttpClientHandler handler = new HttpClientHandler();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri($"https://{location}.azuredatabricks.net/api/");
        }

        public async Task<List<Token>> List()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "2.0/token/list");

                var response = client.SendAsync(request).Result;

                var jsonResponse = await response.Content.ReadAsStringAsync();

                return JObject.Parse(jsonResponse)["token_infos"].ToString().Deserialize<List<Token>>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
