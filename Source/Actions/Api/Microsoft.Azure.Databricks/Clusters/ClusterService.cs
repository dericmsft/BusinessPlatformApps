using Microsoft.Azure.Databricks.Common;
using Microsoft.Azure.Databricks.Model;
using Microsoft.Azure.Databricks.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Clusters
{
    public class ClusterService
    {

        string token;
        HttpClient client;

        public ClusterService(string location, string token)
        {
            this.token = token;

            HttpClientHandler handler = new HttpClientHandler();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri($"https://{location}.azuredatabricks.net/api/");
        }

        public async Task<Cluster> Get(string clusterId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"2.0/clusters/get?cluster_id={clusterId}");

                var response = client.SendAsync(request).Result;

                var jsonResponse = await response.Content.ReadAsStringAsync();

                Cluster c = jsonResponse.Deserialize<Cluster>();

                return c;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Cluster>> List()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "2.0/clusters/list");

                var response = client.SendAsync(request).Result;

                var jsonResponse = await response.Content.ReadAsStringAsync();

                return JObject.Parse(jsonResponse)["clusters"].ToString().Deserialize<List<Cluster>>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> Create(string clusterName)
        {
            try
            {
                string stringPayload = "{  \"cluster_name\": \"__clusterName__\",  \"spark_version\": \"4.0.x-scala2.11\",  \"node_type_id\": \"Standard_D3_v2\",  \"spark_conf\": {    \"spark.speculation\": true  },  \"num_workers\": 2}";

                var httpContent = new StringContent(stringPayload.Replace("__clusterName__", clusterName), Encoding.UTF8, "application/json");

                var response = client.PostAsync("2.0/clusters/create", httpContent).Result;

                var jsonResponse = await response.Content.ReadAsStringAsync();

                dynamic data = JObject.Parse(jsonResponse);

                return data.cluster_id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
