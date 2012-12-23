using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeteSake.SqlSchema
{
    public class SqlTypeInfo
    {
        public SqlTypeInfo(Type type,Type nullableType = null)
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
                    {"smallint", new SqlTypeInfo(typeof(short), typeof(short?))},
                    {"tinyint", new SqlTypeInfo(typeof(byte), typeof(byte?))},
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
                    {"decimal", new SqlTypeInfo(typeof (decimal), typeof (decimal?))},
                    {"date", dateTimeTypeInfo},
                    {"smalldatetime", dateTimeTypeInfo},
                    {"datetime", dateTimeTypeInfo},
                    {"datetime2", dateTimeTypeInfo},
                    {"datetimeoffset", new SqlTypeInfo(typeof(DateTimeOffset), typeof(DateTimeOffset?))},
                    {"timestamp", byteArrayTypeInfo}
                };
        }

        public string GetTypeExpression(bool isNullable)
        {
            var result = isNullable ? CsNullableType : CsType;

            if (result.IsGenericType && result.Name.StartsWith("Nullable"))
            {
                return result.GenericTypeArguments[0].Name + "?";
            }
            else
            {
                return result.Name;
            }
        }
    }
}
