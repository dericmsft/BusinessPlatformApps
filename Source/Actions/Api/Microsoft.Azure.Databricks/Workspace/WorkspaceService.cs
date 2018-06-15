using Microsoft.Azure.Databricks.Common;
using Microsoft.Azure.Databricks.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Workspace
{
    public class WorkspaceService
    {
        string token;
        HttpClient client;

        public WorkspaceService(string location, string token)
        {
            this.token = token;

            HttpClientHandler handler = new HttpClientHandler();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri($"https://{location}.azuredatabricks.net/api/");
        }

        public async Task<bool> WorkspaceExists()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "2.0/token/list");

                var response = client.GetAsync("2.0/workspace/list?path=/Shared/ ").Result;

                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> NotebookImport(NotebookImport import)
        {
            try
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(import, new JsonSerializerSettings() { ContractResolver = new UnderscorePropertyNamesContractResolver() }));

                var response = client.PostAsync("2.0/workspace/import", httpContent).Result;

                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
