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
            this.HasRequiredOption("=", "Base filename used to store query results.", v => BaseFilepath = v);
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
                Query = File.ReadAllText(new Uri(Query).LocalPath);
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

                            foreach(var attribute in xmlElement.Attributes())
                            {
                                if (attribute.Name.LocalName.StartsWith("@"))
                                {
                                    attribute.Remove();
                                    xmlElement.Add(new XAttribute(attribute.Name.LocalName.TrimStart('@'), attribute.Value));
                                }
                            }

                            var rowName = xmlElement.Name.LocalName;

                            if (!openFiles.ContainsKey(rowName))
                            {
                                openFiles[rowName] = new StreamWriter(File.OpenWrite(BaseFilepath + "." + rowName + ".json.txt"));
                                openFiles[rowName].WriteLine("[");
                            }

                            openFiles[rowName].WriteLine(JsonConvert.SerializeXNode(xmlElement, Formatting.Indented, true) + ",");
                            //openFiles[rowName].WriteLine(xmlElement + ",");
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
