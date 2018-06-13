using Microsoft.Azure.Databricks.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Utility
{
    public static class StringUtils
    {
        static JsonSerializerSettings settings;

        static StringUtils()
        {
            settings = new JsonSerializerSettings()
            {
                ContractResolver = new UnderscorePropertyNamesContractResolver()
            };
        }


        public static T Deserialize<T>(this string jsonString)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString, settings);
        }
    }
}
