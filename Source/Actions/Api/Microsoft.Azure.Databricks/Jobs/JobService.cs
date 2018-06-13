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

namespace Microsoft.Azure.Databricks.Jobs
{
    public class JobService
    {
        string token;
        HttpClient client;

        public JobService(string location, string token)
        {
            this.token = token;

            HttpClientHandler handler = new HttpClientHandler();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri($"https://{location}.azuredatabricks.net/api/");
        }

        public async Task<int> Run(int jobId)
        {
            var runJson = "{ \"job_id\": __jobid__ }";

            runJson = runJson.Replace("__jobid__", jobId.ToString());

            try
            {
                var httpContent = new StringContent(runJson, Encoding.UTF8, "application/json");

                var response = client.PostAsync("2.0/jobs/run-now", httpContent).Result;

                var jsonString = await response.Content.ReadAsStringAsync();

                dynamic data = JObject.Parse(jsonString);

                return data.run_id;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<int> Create(string clusterId, string notebookFullPath)
        {
            var jobCreationJson = "{ \"name\": \"6th File Generation\", \"existing_cluster_id\": \"__cluster_id__\", \"libraries\": [], \"timeout_seconds\": 3600, \"max_retries\": 1, \"notebook_task\": { \"notebook_path\": \"__notebook_path__\" } }";

            var jobJson = jobCreationJson.Replace("__notebook_path__", notebookFullPath).Replace("__cluster_id__", clusterId);

            try
            {
                var httpContent = new StringContent(jobJson, Encoding.UTF8, "application/json");

                var response = client.PostAsync("2.0/jobs/create", httpContent).Result;

                var jsonString = await response.Content.ReadAsStringAsync();

                dynamic data = JObject.Parse(jsonString);

                return data.job_id;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<RunState> GetRunState(int runId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"2.0/jobs/runs/get?run_id={runId}");

                var response = client.SendAsync(request).Result;

                var jsonResponse = await response.Content.ReadAsStringAsync();

                dynamic data = JObject.Parse(jsonResponse);

                var strState = data.state.ToString();

                RunState state = StringUtils.Deserialize<RunState>(strState);

                return state;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
