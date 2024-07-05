using System.Data;

namespace Silk.ORM
{
    public static class DBTypeConverter
    {
        private static readonly Dictionary<Type, DbType> typeMap;

        static DBTypeConverter()
        {
            typeMap = new Dictionary<Type, DbType>
            {
                [typeof(byte)] = DbType.Byte,
                [typeof(sbyte)] = DbType.SByte,
                [typeof(short)] = DbType.Int16,
                [typeof(ushort)] = DbType.UInt16,
                [typeof(int)] = DbType.Int32,
                [typeof(uint)] = DbType.UInt32,
                [typeof(long)] = DbType.Int64,
                [typeof(ulong)] = DbType.UInt64,
                [typeof(float)] = DbType.Single,
                [typeof(double)] = DbType.Double,
                [typeof(decimal)] = DbType.Decimal,
                [typeof(bool)] = DbType.Boolean,
                [typeof(string)] = DbType.String,
                [typeof(char)] = DbType.StringFixedLength,
                [typeof(Guid)] = DbType.Guid,
                [typeof(DateTime)] = DbType.DateTime,
                [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof(byte[])] = DbType.Binary,
                [typeof(byte?)] = DbType.Byte,
                [typeof(sbyte?)] = DbType.SByte,
                [typeof(short?)] = DbType.Int16,
                [typeof(ushort?)] = DbType.UInt16,
                [typeof(int?)] = DbType.Int32,
                [typeof(uint?)] = DbType.UInt32,
                [typeof(long?)] = DbType.Int64,
                [typeof(ulong?)] = DbType.UInt64,
                [typeof(float?)] = DbType.Single,
                [typeof(double?)] = DbType.Double,
                [typeof(decimal?)] = DbType.Decimal,
                [typeof(bool?)] = DbType.Boolean,
                [typeof(char?)] = DbType.StringFixedLength,
                [typeof(DateTime?)] = DbType.DateTime,
                [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
                [typeof(object)] = DbType.Object
            };
        }

        public static DbType GetDbType(Type type)
        {
            if (typeMap.TryGetValue(type, out DbType dbType))
            {
                return dbType;
            }
            throw new ArgumentException($"Unsupported type: {type.FullName}");
        }
    }

    public static class  SQLiteTypeConverter
    {
        private static readonly Dictionary<Type, string> typeMap;

        static SQLiteTypeConverter() {
            typeMap = new Dictionary<Type, string>
            {
                [typeof(byte)] = "INTEGER",
                [typeof(sbyte)] = "INTEGER",
                [typeof(short)] = "INTEGER",
                [typeof(ushort)] = "INTEGER",
                [typeof(int)] = "INTEGER",
                [typeof(uint)] = "INTEGER",
                [typeof(long)] = "INTEGER",
                [typeof(ulong)] = "INTEGER",
                [typeof(float)] = "REAL",
                [typeof(double)] = "REAL",
                [typeof(decimal)] = "NUMERIC",
                [typeof(bool)] = "INTEGER",
                [typeof(string)] = "TEXT",
                [typeof(char)] = "TEXT",
                [typeof(Guid)] = "TEXT",
                [typeof(DateTime)] = "TEXT",
                [typeof(DateTimeOffset)] = "TEXT",
                [typeof(byte[])] = "BLOB",
                [typeof(byte?)] = "INTEGER",
                [typeof(sbyte?)] = "INTEGER",
                [typeof(short?)] = "INTEGER",
                [typeof(ushort?)] = "INTEGER",
                [typeof(int?)] = "INTEGER",
                [typeof(uint?)] = "INTEGER",
                [typeof(long?)] = "INTEGER",
                [typeof(ulong?)] = "INTEGER",
                [typeof(float?)] = "REAL",
                [typeof(double?)] = "REAL",
                    
                [typeof(decimal?)] = "NUMERIC",
                [typeof(bool?)] = "INTEGER",
                [typeof(char?)] = "TEXT",
                [typeof(DateTime?)] = "TEXT",
                [typeof(DateTimeOffset?)] = "TEXT",
                [typeof(object)] = "BLOB"
            };
        }

        public static string GetDbType(Type type)
        {
            if (typeMap.TryGetValue(type, out string dbType))
            {
                return dbType;
            }
            throw new ArgumentException($"Unsupported type: {type.FullName}");
        }

    }
}
