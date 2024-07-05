using System.Data;
using System.Data.SQLite;

namespace Silk.ORM
{
    /// <summary>
    /// SQLiteContext class, inherits from DatabaseContext <see cref="DatabaseContext{T}"/>
    /// Create a Context of SQLiteConnection, and implements the methods to execute queries and non queries
    /// 
    /// <para> the param of new SQLiteContext is the connection string, if is null, the connection string is <c>"Data Source=:memory:;Version=3;New=True;"</c></para>
    /// </summary>
    public class SQLiteContext : DatabaseContext<SQLiteConnection>
    {
        public SQLiteContext(string? connectionString) : base((connectionString != null) ? connectionString : "Data Source=:memory:;Version=3;New=True;")
        {}

        /// <summary>
        /// Execute a non query in the database
        /// <param name="query"> SQL query with reference to parameters </param>
        /// <param name="parameters"> parameters to add to the query </param>
        /// <example>
        /// <code>
        /// //if you want to insert a user in the table users with a pure string.
        /// var sql = "INSERT INTO users (name, age) VALUES (@name, @age)";
        /// parameters = new { name = "silk user", age = 29 };
        /// //You can use Sql handler to create the query
        /// </code>
        /// </example>
        /// It is only a example, you can use the query that you want
        /// But it is handle by Repository class <see cref="Repository{T,repositoryType}"/>
        /// <returns> 
        /// A integer of result Execution 
        /// </returns>
        /// </summary>
        public override int ExecuteNonQuery(string query, object parameters = null)
        {
            var command = Use().CreateCommand();
            command.CommandText = query;
            AddParameters(command, parameters);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a query in the database, and return a IEnumerable of the result.
        /// The result is a object of the type that you want to return.
        /// <param name="query"> SQL query with reference to parameters </param>
        /// <param name="parameters"> parameters to add to the query </param>
        /// It is only a example, you can use the query that you want
        /// <returns> 
        /// Return a IEnumerable of the result of the query of the type that you want
        /// </returns>
        /// </summary>
        public override IEnumerable<LT> ExecuteQuery<LT>(string query, object parameters = null)
        {
            using (var cmd = Use().CreateCommand())
            {
                cmd.CommandText = query;
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapObject<LT>(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Execute a query in the database, and return a single build of the result.
        /// The result is a object of the type that you want to return.
        /// <param name="query"> SQL query with reference to parameters </param>
        /// <param name="parameters"> parameters to add to the query </param>
        /// <example>
        /// <code>
        /// 
        /// </code>
        /// </example>
        /// It is only a example, you can use the query that you want
        /// <returns> 
        /// Return a single build of the result of the query of the type that you want
        /// </returns>
        /// </summary>
        public override LT ExecuteSingleQuery<LT>(string query, object parameters = null)
        {

            using (var cmd = Use().CreateCommand())
            {
                cmd.CommandText = query;
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapObject<LT>(reader);
                    }
                }

            }
            return default(LT)!;
        }

        /// <summary>
        /// Execute a query in the database, and return a single build of the result.
        /// The result is a object of anonymous type.
        /// <param name="query"> SQL query with reference to parameters </param>
        /// <param name="parameters"> parameters to add to the query </param>
        /// <example>
        /// <code>
        /// 
        /// </code>
        /// </example>
        /// It is only a example, you can use the query that you want
        /// <returns> 
        /// Return a single build of the result of the query but in anonymous type
        /// </returns>
        /// </summary>
        public override object ExecuteSingleQueryAnonymous(string query, object parameters = null)
        {
            using (var cmd = Use().CreateCommand())
            {
                cmd.CommandText = query;
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return DataReaderToObject(reader);
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Execute a query in the database, and return a IEnumerable of the result.
        /// The result is a object of anonymous type.
        /// It is only a example, you can use the query that you want.
        /// <param name="query"> SQL query with reference to parameters </param>
        /// <param name="parameters"> parameters to add to the query </param>
        /// <returns> 
        /// Return a IEnumerable of the result of the query but in anonymous type
        /// </returns>
        /// </summary>
        public override IEnumerable<object> ExecuteQueryAnonymous(string query, object parameters = null)
        {
            using (var cmd = Use().CreateCommand())
            {
                cmd.CommandText = query;
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return DataReaderToObject(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Use the connection of the database, if the connection is null, throw a exception, if the connection is open, return the connection
        /// <para>
        /// </para>
        /// This method open the connection of the database but not close it, you need to close it manually.
        /// or use in <c>using</c> statement.
        /// </summary>
        public override SQLiteConnection Use()
        {
            if (DbConnection == null)
            {
                throw new Exception("Database connection is null");
            }

            if (DbConnection.State == ConnectionState.Open)
            {
                return DbConnection;
            }
            DbConnection.ConnectionString = ConnectionString;
            DbConnection.Open();
            return DbConnection;
        }
        /// <summary>
        /// Close the current connection of the database.
        /// If the connection is a <c>:memory:</c> database, the database is deleted.
        /// </summary>
        public override void Close()
        {
            if (DbConnection?.State == ConnectionState.Open)
            {
                DbConnection.Close();
            }
        }
    }
}
