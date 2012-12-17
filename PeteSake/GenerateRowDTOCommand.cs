using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManyConsole;
using Newtonsoft.Json;
using PeteSake.SqlSchema;

namespace PeteSake
{
    public class GenerateRowDTOCommand : ConsoleCommand
    {
        SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder()
        {
            DataSource = ".\\SQLEXPRESS",
            InitialCatalog = "",
            IntegratedSecurity = true
        }; 

        public GenerateRowDTOCommand()
        {
            this.IsCommand("generate-row-dto", "Generates row DTO from a SQL schema.");
            this.HasOption("c=", "SQL connection string (overwrites any sql parameters already set", v => connectionString = new SqlConnectionStringBuilder(v));
            this.HasOption("server=", "Name of SQL server", v => connectionString.DataSource = v);
            this.HasOption("database=", "Name of SQL database", v => connectionString.InitialCatalog = v);
        }

        public override int Run(string[] remainingArguments)
        {
            var results = SchemaReader.GetTables(connectionString.ToString());
            Console.WriteLine("Tables: " + JsonConvert.SerializeObject(results, Formatting.Indented));

            return 0;
        }
    }
}
