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
        public static List<Table> GetTables(SqlConnectionStringBuilder sqlConnectionStringBuilder)
        {
            List<Table> results = new List<Table>();

            using (var connection = new SqlConnection(sqlConnectionStringBuilder.ToString()))
            {
                connection.Open();

                DataTable tables = connection.GetSchema("Tables");

                Dictionary<string, int> columnIndex = new Dictionary<string, int>();

                foreach (DataColumn column in  tables.Columns)
                {
                    columnIndex[column.ColumnName] = column.Ordinal;
                }

                foreach (DataRow row in tables.Rows)
                {
                    var table = new Table();

                    table.Catalog = (string) row.ItemArray[columnIndex["TABLE_CATALOG"]];
                    table.Name = (string) row.ItemArray[columnIndex["TABLE_NAME"]];
                    table.Schema = (string) row.ItemArray[columnIndex["TABLE_SCHEMA"]];
                    table.Equals(row.ItemArray[columnIndex["TABLE_TYPE"]]);

                    results.Add(table);
                }
            }
            return results;
        }
    }
}
