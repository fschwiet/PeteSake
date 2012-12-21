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

        public static Dictionary<string, SqlTypeInfo> GetTypeMapping()
        {
            var dateTimeTypeInfo = new SqlTypeInfo(typeof (DateTime), typeof (DateTime?));
            var intTypeInfo = new SqlTypeInfo(typeof (int), typeof (int?));
            var byteArrayTypeInfo = new SqlTypeInfo(typeof (byte[]));

            return new Dictionary<string, SqlTypeInfo>()
                {
                    {"int", intTypeInfo},
                    {"smallint", intTypeInfo},
                    {"tinyint", intTypeInfo},
                    {"varchar", new SqlTypeInfo(typeof (string))},
                    {"nvarchar", new SqlTypeInfo(typeof (string))},
                    {"text", new SqlTypeInfo(typeof (string))},
                    {"ntext", new SqlTypeInfo(typeof (string))},
                    {"xml", new SqlTypeInfo(typeof (string))},
                    {"bigint", new SqlTypeInfo(typeof (long), typeof (long?))},
                    {"uniqueidentifier", new SqlTypeInfo(typeof (Guid), typeof (Guid?))},
                    {"bit", new SqlTypeInfo(typeof (bool), typeof (bool?))},
                    {"binary", byteArrayTypeInfo},
                    {"varbinary", byteArrayTypeInfo},
                    {"image", byteArrayTypeInfo},
                    {"decimal", new SqlTypeInfo(typeof (double), typeof (double?))},
                    {"date", dateTimeTypeInfo},
                    {"smalldatetime", dateTimeTypeInfo},
                    {"datetime", dateTimeTypeInfo},
                    {"datetime2", dateTimeTypeInfo},
                    {"datetimeoffset", dateTimeTypeInfo},
                    {"timestamp", new SqlTypeInfo(typeof(ulong), typeof(ulong?))}
                };
        }
    }
}
