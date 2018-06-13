using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Model
{
    public enum ClusterState
    {
        PENDING = 1,
        RUNNING = 2,
        RESTARTING = 3,
        RESIZING = 4,
        TERMINATING = 5,
        TERMINATED = 6,
        ERROR = 7,
        UNKNOWN = 8
    }
}
