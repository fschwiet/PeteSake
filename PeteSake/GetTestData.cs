using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ManyConsole;

namespace PeteSake
{
    public class GetTestData : ConsoleCommand
    {
        public GetTestData()
        {
            this.IsCommand("save-test-json", "Downloads SQL results and writes them to json.");
            this.HasRequiredOption("p=", "Base filename used to store query results.", v => BaseFilepath = v);
            this.HasRequiredOption("c=", "Connection string with default database set.", v => SqlConnectionString = v);
            this.HasRequiredOption("q=", "Query to execute (filepath or actual query text).", v => Query = v);
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

            using(var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();

                using(var command = new SqlCommand(Query, connection))
                {
                    var result = command.ExecuteXmlReader();

                    var doc = XDocument.Load(result);

                    Console.WriteLine(doc.ToString());
                }
            }

            return 0;
        }
    }
}
