using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeteSake.SqlSchema
{
    public class Column
    {
        public string Name;
        public bool IsNullable;
        public int? PrimaryKeyPosition;
        public string Type;
    }
}
