using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Model
{
    public class Cluster
    {
        public string ClusterId { get; set; }
        public string CreatorUserName { get; set; }
        public Int64 SparkContextId { get; set; }
        public string ClusterName { get; set; }
        public int AutoterminationMinutes { get; set; }
        public ClusterState State { get; set; }
        public string StateMessage { get; set; }
        public Int64 StartTime { get; set; }
        public Int64 TerminatedTime { get; set; }
    }
}
