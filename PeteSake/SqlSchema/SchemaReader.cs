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

            VisitSchemaRows(connectionString, "Tables", (source,keys) =>
            {
                var table = new Table();

                table.Catalog = (string)source("TABLE_CATALOG");
                table.Name = (string)source("TABLE_NAME");
                table.Schema = (string)source("TABLE_SCHEMA");
                table.Equals(source("TABLE_TYPE"));

                results.Add(table);
                tableById[table.Id] = table;
            });

            VisitSchemaRows(connectionString, "Columns", (source,keys) =>
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

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var table in tableById.Values)
                {
                    using (var command = new SqlCommand("sp_pkeys", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@table_name", table.Name));

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                                var indexPosition = reader.GetInt16(reader.GetOrdinal("KEY_SEQ"));
                                table.Columns.Single(c => c.Name == columnName).PrimaryKeyPosition = indexPosition;
                            }
                        }
                    }
                }
            }

            return results;
        }

        private static void VisitSchemaRows(string connectionString, string schemaElement, Action<Func<string, object>, string[]> rowHandler)
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
                    rowHandler(key => row.ItemArray[columnIndex[key]], columnIndex.Keys.ToArray());
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
