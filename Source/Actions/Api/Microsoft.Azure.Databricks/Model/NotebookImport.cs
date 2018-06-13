using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Model
{
    public class NotebookImport
    {
        public  string Content { get; set; }
        public string Path { get; set; }
        public string Language { get; set; }
        public bool Overwrite { get; set; }
        public ExportFormat Format { get; set; }
    }
}
