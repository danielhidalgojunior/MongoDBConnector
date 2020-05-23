using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBConnector
{
    public class JsonConfigurator
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public static JsonConfigurator GetConnectionStringFromConfigFile(string file = @"config\db.json")
        {
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                var obj = (JObject)JsonConvert.DeserializeObject(json);

                return new JsonConfigurator
                {
                    ConnectionString = (string)obj["connectionString"],
                    DatabaseName = (string)obj["databaseName"]
                };
            }
            else
                return null;
        }
    }
}
