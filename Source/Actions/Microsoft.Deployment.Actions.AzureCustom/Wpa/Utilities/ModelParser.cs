using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace Microsoft.Deployment.Actions.AzureCustom.Wpa.Utilities
{
    public class ModelParser
    {

        string storageAccountName;
        string authenticationKey;
        string containerName;
        string folderName;
        string modelString;
        public ModelParser(string jsonModel, string storageAccountName, string authenticationKey, string containerName, string folderName)
        {
            this.modelString = jsonModel;
            this.storageAccountName = storageAccountName;
            this.authenticationKey = authenticationKey;
            this.containerName = containerName;
            this.folderName = folderName;
        }

        public string Parse()
        {
            string folderNameWithSlash = folderName + "/";
            string folderNameWithPrefixAndSlash = "_" + folderNameWithSlash;

            modelString = modelString.Replace("__storageAccountName__", storageAccountName);
            modelString = modelString.Replace("********", authenticationKey);
            modelString = modelString.Replace("__authenticationKey__", authenticationKey);
            modelString = modelString.Replace("__containername__", containerName);
            modelString = modelString.Replace("__folderNameWithSlash__", folderNameWithSlash);
            modelString = modelString.Replace("__folderNameWithPrefixAndSlash__", folderNameWithPrefixAndSlash);

            return modelString;
        }

        public static string AddColumns(string jsonModel, string tableName, string jsonColumns)
        {
            var dbWithTable = TOM.JsonSerializer.DeserializeDatabase(jsonModel);

            var table = dbWithTable.Model.Tables[tableName];

            var colArray = JArray.Parse(jsonColumns);

            if (table != null)
            {
                foreach (var item in colArray)
                {
                    TOM.Column column = TOM.JsonSerializer.DeserializeObject<TOM.Column>(item.ToString());

                    table.Columns.Add(column);
                }
            }

            TOM.Partition partition = table.Partitions.FirstOrDefault();

            TOM.MPartitionSource mPartitionSource = (TOM.MPartitionSource)partition.Source;

            var personHistoricalColumn = table.Columns;

            mPartitionSource.Expression = GetPartitionExpression("PersonHistorical", table.Columns);

            return TOM.JsonSerializer.SerializeDatabase(dbWithTable, new TOM.SerializeOptions() { SplitMultilineStrings = true });
        }

        private static string GetPartitionExpression(string tableName, TOM.ColumnCollection columns)
        {
            StringBuilder b = new StringBuilder();
            b.Append("let\n");
            b.Append("    Source = #\"AzureBlobs/https://__storageAccountName__ blob core windows net/\",\n");
            b.Append("    __containername__1 = Source{[Name=\"");
            b.Append("__containername__");
            b.Append("\"]}[Data],\n");
            b.Append("    #\"https://__storageAccountName__ blob core windows net/__containername__/___tableName__ csv\" = __containername__1{[#\"Folder Path\"");
            b.Append("=\"https://");
            b.Append("__storageAccountName__");
            b.Append(".blob.core.windows.net/");
            b.Append("__containername__");
            b.Append("/\",Name=\"");
            b.Append("__tableName__.csv");
            b.Append("\"]}[Content],\n");
            b.Append("    #\"Imported CSV\" = Csv.Document(#\"https://__storageAccountName__ blob core windows net/__containername__/___tableName__ csv\",[Delimiter=\",\", Columns=19, Encoding=1252, QuoteStyle=QuoteStyle.None]),\n");
            b.Append("    #\"Promoted Headers\" = Table.PromoteHeaders(#\"Imported CSV\", [PromoteAllScalars=true]),\n");
            b.Append("    #\"Changed Type\" = Table.TransformColumnTypes(#\"Promoted Headers\",{");
            b.Append(GetTablePowerQuerySchema(columns));
            b.Append("})\n");
            b.Append("in\n");
            b.Append("    #\"Changed Type\"");



            return b.ToString().Replace("__tableName__", tableName);
        }

        private static string GetTablePowerQuerySchema(TOM.ColumnCollection tableColumns)
        {
            return string.Join(
                ", ",
                tableColumns
                    .Select(
                        column => $"{{\"{column.Name}\", {GetPowerQueryType(column.DataType)}}}"));
        }

        public static string GetPowerQueryType(TOM.DataType tabularType)
        {
            return TabularTypeToPowerQueryTypeMapping.ContainsKey(tabularType)
                ? TabularTypeToPowerQueryTypeMapping[tabularType]
                : TabularTypeToPowerQueryTypeMapping[TOM.DataType.String];
        }

        private static IDictionary<TOM.DataType, string> TabularTypeToPowerQueryTypeMapping { get; } = new Dictionary<TOM.DataType, string>
        {
            { TOM.DataType.String, "type text" },
            { TOM.DataType.DateTime, "type datetime" },
            { TOM.DataType.Int64, "Int64.Type" },
            { TOM.DataType.Double, "type number" },
            { TOM.DataType.Decimal, "type number" },
            { TOM.DataType.Boolean, "type logical" }
        };
    }

}
