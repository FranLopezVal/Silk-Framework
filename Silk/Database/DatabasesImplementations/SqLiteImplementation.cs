using System.Data.SQLite;
using Silk.SQL;

namespace Silk.Database
{
    [DatabasePlugin("sqlite", null, "sqlite")]
    public class SqLiteImplementation : IDatabaseConnection
    {
        private string connectionString = "Data Source=:memory:;Version=3;New=True;";
        private SQLiteConnection? connection;

        public void Close()
        {
            if (connection == null) return;
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public void ExecuteQuery(string query)
        {
            if (connection == null) return;
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
        }

        public void ExecuteQuery(Sql sQL)
        {
            ExecuteQuery(sQL.ToString());
        }

        public T ExecuteQuery<T>(string query) where T : new()
        {
            if (connection == null) return new T();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = query;
                using (var reader = cmd.ExecuteReader())
                {
                    var obj = new T();
                    var properties = typeof(T).GetProperties();
                    while (reader.Read())
                    {
                        foreach (var prop in properties)
                        {
                            if (reader[prop.Name] != DBNull.Value)
                            {
                                prop.SetValue(obj, reader[prop.Name]);
                            }
                        }
                    }
                    return obj;
                }
            }
        }

        public T ExecuteQuery<T>(Sql query) where T : new()
        {
            return ExecuteQuery<T>(query.ToString());
        }

        public void Open()
        {
            connection = Open<SQLiteConnection>();
            connection.ConnectionString = connectionString;
            connection.Open();
        }

        public T Open<T>() where T : new()
        {
            var conn = new T();
            return conn;
        }
    }
}
