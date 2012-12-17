using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeteSake.SqlSchema
{
    public class SqlTypeInfo
    {
        public SqlTypeInfo(Type type, Type nullableType = null)
        {
            CsType = type;
            CsNullableType = nullableType ?? type;
        }

        public Type CsType;
        public Type CsNullableType;
    }
}
