using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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

        public string Directory;
        public string Namespace;

        public GenerateRowDTOCommand()
        {
            this.IsCommand("generate-row-dto", "Generates row DTO from a SQL schema.");
            this.HasOption("c=", "SQL connection string (overwrites any sql parameters already set", v => connectionString = new SqlConnectionStringBuilder(v));
            this.HasOption("server=", "Name of SQL server", v => connectionString.DataSource = v);
            this.HasOption("database=", "Name of SQL database", v => connectionString.InitialCatalog = v);
            this.HasRequiredOption("d=", "Output directory to write classes to", v => Directory = v);
            this.HasRequiredOption("n=", "Namespace to write classes to", v => Namespace = v);
        }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            if (string.IsNullOrEmpty(connectionString.InitialCatalog))
            {
                throw new ConsoleHelpAsException("A database/catalog must be specified in the connection string.");
            }
            
            return base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
        }

        Dictionary<string, SqlTypeInfo> typeMapping = GetTypeMapping(); 

        public override int Run(string[] remainingArguments)
        {
            var results = SchemaReader.GetTables(connectionString.ToString());
            Console.WriteLine("Tables: " + JsonConvert.SerializeObject(results, Formatting.Indented));

            foreach (var table in results)
            {
                var className = (table.Schema + "." + table.Name).Replace(".", "_");

                var file = Path.Combine(Directory, className + ".cs");

                using(var stream = new FileStream(file, FileMode.OpenOrCreate))
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine("using System;");
                    writer.WriteLine();
                    writer.WriteLine("namespace " + Namespace);
                    writer.WriteLine("{");
                    writer.WriteLine("    public partial class " + className);
                    writer.WriteLine("    {");

                    foreach (var column in table.Columns)
                    {
                        SqlTypeInfo type;

                        if (!typeMapping.TryGetValue(column.Type, out type))
                            throw new Exception("Unable to convert SQL type to .NET type: " + column.Type);

                        writer.WriteLine("         public {0} {1};", column.IsNullable ? type.CsNullableType : type.CsType, column.Name);    
                    }

                    writer.WriteLine("    }");
                    writer.WriteLine("}");
                }
            }

            return 0;
        }

        private static Dictionary<string, SqlTypeInfo> GetTypeMapping()
        {
            var dateTimeTypeInfo = new SqlTypeInfo(typeof (DateTime), typeof (DateTime?));
            var intTypeInfo = new SqlTypeInfo(typeof (int), typeof (int?));

            return new Dictionary<string, SqlTypeInfo>()
            {
                {"int", intTypeInfo},
                {"varchar", new SqlTypeInfo(typeof (string))},
                {"nvarchar", new SqlTypeInfo(typeof (string))},
                {"text", new SqlTypeInfo(typeof (string))},
                {"ntext", new SqlTypeInfo(typeof (string))},
                {"xml", new SqlTypeInfo(typeof (string))},
                {"bigint", new SqlTypeInfo(typeof (long), typeof (long?))},
                {"uniqueidentifier", new SqlTypeInfo(typeof (Guid), typeof (Guid?))},
                {"bit", new SqlTypeInfo(typeof (bool), typeof (bool?))},
                {"binary", new SqlTypeInfo(typeof (byte[]))},
                {"varbinary", new SqlTypeInfo(typeof (byte[]))},
                {"image", new SqlTypeInfo(typeof (byte[]))},
                {"decimal", new SqlTypeInfo(typeof (double), typeof (double?))},
                {"date", dateTimeTypeInfo},
                {"smalldatetime", dateTimeTypeInfo},
                {"datetime", dateTimeTypeInfo},
                {"datetime2", dateTimeTypeInfo},
                {"datetimeoffset", dateTimeTypeInfo},
                {"smallint", intTypeInfo},
                {"tinyint", intTypeInfo},
                {"timestamp", new SqlTypeInfo(typeof(ulong), typeof(ulong?))}
            };
        }
    }
}
