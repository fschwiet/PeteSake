using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeteSake.SqlSchema
{
    public class SchemaReader
    {
        public static List<Table> GetTables(string connectionString)
        {
            List<Table> results = new List<Table>();
            Dictionary<string,Table> tableById = new Dictionary<string, Table>();

            VisitSchemaRows(connectionString, "Tables", source =>
            {
                var table1 = new Table();

                table1.Catalog = (string)source("TABLE_CATALOG");
                table1.Name = (string)source("TABLE_NAME");
                table1.Schema = (string)source("TABLE_SCHEMA");
                table1.Equals(source("TABLE_TYPE"));

                results.Add(table1);
                tableById[table1.Id] = table1;
            });

            VisitSchemaRows(connectionString, "Columns", source =>
            {
                var tableId = Table.GetDatabaseKey((string)source("TABLE_SCHEMA"),
                                                   (string)source("TABLE_NAME"));

                var table = tableById[tableId];

                var column = new Column();
                column.Name = (string)source("COLUMN_NAME");
                column.IsNullable = YesNoToBool((string)source("IS_NULLABLE"));
                column.Type = (string) source("DATA_TYPE");

                table.Columns.Add(column);
            });

            return results;
        }

        private static void VisitSchemaRows(string connectionString, string schemaElement, Action<Func<string,object>> rowHandler)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DataTable tables = connection.GetSchema(schemaElement);

                Dictionary<string, int> columnIndex = new Dictionary<string, int>();

                foreach (DataColumn column in  tables.Columns)
                {
                    columnIndex[column.ColumnName] = column.Ordinal;
                }

                foreach (DataRow row in tables.Rows)
                {
                    rowHandler(key => row.ItemArray[columnIndex[key]]);
                }
            }
        }

        private static bool YesNoToBool(string input)
        {
            if (input.Equals("yes", StringComparison.OrdinalIgnoreCase))
                return true;
            else if (input.Equals("no", StringComparison.OrdinalIgnoreCase))
                return false;
            else
                throw new Exception("Could not convert value to bool: " + input);
        }
    }
}
