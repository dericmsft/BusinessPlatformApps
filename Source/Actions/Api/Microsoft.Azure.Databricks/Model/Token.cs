using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Model
{
    public class Token
    {
        public string TokenId { get; set; }
        public Int64 CreationTime { get; set; }
        public Int64 ExpiryTime { get; set; }
        public string Comment { get; set; }
    }
}
