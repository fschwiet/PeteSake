using System;
using System.Collections.Generic;

namespace PeteSake.SqlSchema
{
    public class Table
    {
        public string Catalog;
        public string Schema;
        public string Name;
        public TableType Type;
        public List<Column> Columns = new List<Column>();

        public string Id
        {
            get { return GetDatabaseKey(Schema, Name); }
        }

        public static string GetDatabaseKey(string schema, string name)
        {
            return schema + "." + name;
        }

        public TableType SetType(string type)
        {
            switch (type)
            {
            case "BASE TABLE": return TableType.Table;
            case "VIEW": return TableType.View;
            default: return (TableType)Enum.Parse(typeof(TableType), type, true);
            }
        }
    }
}