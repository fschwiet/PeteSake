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

            VisitSchemaRows(connectionString, "Tables", (row, columnIndex) =>
            {
                var table1 = new Table();

                table1.Catalog = (string)row.ItemArray[columnIndex["TABLE_CATALOG"]];
                table1.Name = (string)row.ItemArray[columnIndex["TABLE_NAME"]];
                table1.Schema = (string)row.ItemArray[columnIndex["TABLE_SCHEMA"]];
                table1.Equals(row.ItemArray[columnIndex["TABLE_TYPE"]]);

                results.Add(table1);
                tableById[table1.Id] = table1;
            });

            VisitSchemaRows(connectionString, "Columns", (row, columnIndex) =>
            {
                var tableId = Table.GetDatabaseKey((string)row.ItemArray[columnIndex["TABLE_SCHEMA"]],
                                                   (string)row.ItemArray[columnIndex["TABLE_NAME"]]);

                var table = tableById[tableId];

                var column = new Column();
                column.Name = (string)row.ItemArray[columnIndex["COLUMN_NAME"]];
                column.IsNullable = YesNoToBool((string)row.ItemArray[columnIndex["IS_NULLABLE"]]);
                column.Type = (string) row.ItemArray[columnIndex["DATA_TYPE"]];
                table.Columns.Add(column);

                Dictionary<string, string> tableColumn = new Dictionary<string, string>();

                foreach (var key in columnIndex.Keys)
                {
                    var value = row.ItemArray[columnIndex[key]];
                    tableColumn[key] = value.GetType().Name + " " + value.ToString();
                }

                Console.WriteLine(JsonConvert.SerializeObject(tableColumn, Formatting.Indented));
            });


            return results;
        }

        private static void VisitSchemaRows(string connectionString, string schemaElement, Action<DataRow, Dictionary<string, int>> rowHandler)
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
                    rowHandler(row, columnIndex);
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
