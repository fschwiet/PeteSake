using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeteSake.SqlSchema
{
    public class SchemaReader
    {
        public static List<Table> GetTables(string connectionString)
        {
            List<Table> results = new List<Table>();
            Dictionary<string,Table> tableById = new Dictionary<string, Table>();

            Action<DataRow, Dictionary<string, int>> rowHandler = (row, columnIndex) =>
            {
                var table = new Table();

                table.Catalog = (string)row.ItemArray[columnIndex["TABLE_CATALOG"]];
                table.Name = (string)row.ItemArray[columnIndex["TABLE_NAME"]];
                table.Schema = (string)row.ItemArray[columnIndex["TABLE_SCHEMA"]];
                table.Equals(row.ItemArray[columnIndex["TABLE_TYPE"]]);

                results.Add(table);
                tableById[table.Id] = table;
            };

            VisitSchemaRows(connectionString, "Tables", rowHandler);
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
    }
}
