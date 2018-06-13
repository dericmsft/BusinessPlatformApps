using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Model
{
    public class RunState
    {
        public RunLifeCycleState LifeCycleState{ get; set; }
        public RunResultState ResultState { get; set; }
        public string StateMessage { get; set; }
    }
}
