using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ManyConsole;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace PeteSake
{
    public class GetTestData : ConsoleCommand
    {
        public GetTestData()
        {
            this.IsCommand("save-test-json", "Downloads SQL results and writes them to json.");
            this.HasRequiredOption("p=", "Base filename used to store query results.", v => BaseFilepath = v);
            this.HasRequiredOption("c=", "Connection string with default database set.", v => SqlConnectionString = v);
            this.HasRequiredOption("q=", "Query to execute (filepath or actual query text) (should produce an XML result -> add 'for XML auto' to the end of SELECT).", v => Query = v);
        }

        public string BaseFilepath;
        public string SqlConnectionString;
        public string Query;

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            BaseFilepath = Path.GetFullPath(BaseFilepath);
            return null;
        }

        public override int Run(string[] remainingArguments)
        {
            try
            {
                Query = File.ReadAllText(Path.GetFullPath(Query));
            }
            catch (Exception)
            {
            }

            Dictionary<string, StreamWriter> openFiles = new Dictionary<string, StreamWriter>();

            using(var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();

                using(var command = new SqlCommand(Query, connection))
                {
                    using(var reader = command.ExecuteXmlReader())
                    {
                        while (reader.Read())
                        {
                            var xmlElement = XElement.Parse(reader.ReadOuterXml());

                            var rowName = xmlElement.Name.LocalName;

                            if (!openFiles.ContainsKey(rowName))
                            {
                                openFiles[rowName] = new StreamWriter(File.Open(BaseFilepath + "." + rowName + ".json.txt", FileMode.OpenOrCreate));
                                openFiles[rowName].WriteLine("[");
                            }

                            Dictionary<string, object> row= new Dictionary<string, object>();

                            foreach (var attribute in xmlElement.Attributes())
                                row[attribute.Name.LocalName] = attribute.Value;

                            openFiles[rowName].WriteLine(JsonConvert.SerializeObject(row, Formatting.Indented) + ",");
                        }
                    }
                }
            }

            foreach(var file in openFiles.Values)
            {
                file.WriteLine("]");
                file.Flush();
                file.Close();
            }

            return 0;
        }
    }
}
