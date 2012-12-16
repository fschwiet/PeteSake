using System;

namespace PeteSake.SqlSchema
{
    public class Table
    {
        public string Catalog;
        public string Schema;
        public string Name;
        public TableType Type;

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