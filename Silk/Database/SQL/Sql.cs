using System.Runtime.Serialization;
using System.Text;

namespace Silk.SQL
{
    public sealed class Sql: ISerializable, IEquatable<Sql>, ICloneable
    {
        private StringBuilder query;
        private bool endWithSemicolon = false;


        public bool HasSemicolonAtEnd => query[^1] == ';';

        private Sql(string command, bool endWithSemicolon = true)
        {
            query = new StringBuilder(command);
            this.endWithSemicolon = endWithSemicolon;
        }

        // Metodos estaticos para CREATE

        public static Sql CreateDatabase(string databaseName)
        {
            return new Sql($"CREATE DATABASE {databaseName}");
        }

        public static Sql CreateTable(string tableName, string columns, bool ifNoExist = false)
        {
            return new Sql($"CREATE TABLE {(ifNoExist ? "IF NOT EXISTS " : "")}{tableName} ({columns})");
        }

        public static Sql CreateTable(string tableName, bool ifNoExist = false)
        {
            return new Sql($"CREATE TABLE {(ifNoExist ? "IF NOT EXISTS " : "")}{tableName} (");
        }

        // Método estático para SELECT
        public static Sql Select(string columns)
        {
            return new Sql($"SELECT {columns}");
        }

        public static Sql SelectDistinct(string columns)
        {
            return new Sql($"SELECT DISTINCT {columns}");
        }

        // Metodo para CREATE (ADD COLUMN)
        public Sql AddColumn( string name,string? type = null,bool primary = false, bool notNull = false, bool unique = false) 
        {

            if (query.ToString()[^1] == ')')
            {
                query.Remove(query.Length - 1, 1);
            }

            if (query.ToString()[^1] == '(')
            {
                query.Append($"{name} {(type != null ? type : "")}");
                if (primary)
                {
                    query.Append(" PRIMARY KEY");
                }
                if (notNull)
                {
                    query.Append(" NOT NULL");
                }
                if (unique)
                {
                    query.Append(" UNIQUE");
                }
                return this;
            }
            else
            {
                query.Append($", {name} {(type != null ? type : "")}");
                if (primary)
                {
                    query.Append(" PRIMARY KEY");
                }
                if (notNull)
                {
                    query.Append(" NOT NULL");
                }
                if (unique)
                {
                    query.Append(" UNIQUE");
                }
                query.Append(") ");
                return this;
            }
        }

        public Sql RefColumn(string name)
        {

            if (query.ToString()[^1] == ')')
            {
                query.Remove(query.Length - 1, 1);
            }

            if (query.ToString()[^1] == '(')
            {
                query.Append($"{name}");

                return this;
            }
            else
            {
                query.Append($", {name}");
                
                query.Append(") ");
                return this;
            }
        }

        // Método para COUNT
        public static Sql Count(string columns)
        {
            return new Sql($"COUNT({columns})");
        }

        public static Sql SelectCount(string columns)
        {
            return new Sql($"SELECT COUNT({columns})");
        }

        // Método para FROM
        public Sql From(string table)
        {
            query.Append($" FROM {table}");
            return this;
        }

        // Método para WHERE
        public Sql Where(string condition)
        {
            query.Append($" WHERE {condition}");
            return this;
        }

        // Métodos para AND y OR
        public Sql And(string condition)
        {
            query.Append($" AND {condition}");
            return this;
        }

        public Sql Or(string condition)
        {
            query.Append($" OR {condition}");
            return this;
        }

        // Método para ORDER BY
        public Sql OrderBy(string columns,string type = "DESC")
        {
            query.Append($" ORDER BY {columns} {type}");
            return this;
        }

        // Método para GROUP BY
        public Sql GroupBy(string columns)
        {
            query.Append($" GROUP BY {columns}");
            return this;
        }

        // Método para HAVING
        public Sql Having(string condition)
        {
            query.Append($" HAVING {condition}");
            return this;
        }

        // Método para JOIN
        public Sql Join(string table, string condition)
        {
            query.Append($" JOIN {table} ON {condition}");
            return this;
        }

        // Métodos para otros tipos de JOIN
        public Sql LeftJoin(string table, string condition)
        {
            query.Append($" LEFT JOIN {table} ON {condition}");
            return this;
        }

        public Sql RightJoin(string table, string condition)
        {
            query.Append($" RIGHT JOIN {table} ON {condition}");
            return this;
        }

        public Sql InnerJoin(string table, string condition)
        {
            query.Append($" INNER JOIN {table} ON {condition}");
            return this;
        }

        // Método para INSERT
        public static Sql InsertInto(string table, string columns, string values)
        {
            return new Sql($"INSERT INTO {table} ({columns}) VALUES ({values}) ");
        }
        public static Sql InsertInto(string table)
        {
            return new Sql($"INSERT INTO {table} (");
        }

        public Sql AddValue(string column, object value)
        {
            if (!query.ToString().Contains("VALUES"))
            {
                if (query.ToString()[^1] == '(')
                {
                    query.Remove(query.Length - 1, 1);
                }

                query.Append("VALUES (");
            }
            if (query.ToString()[^1] == ')')
            {
                query.Remove(query.Length - 1, 1);
            }

            if (query.ToString()[^1] == '(')
            {
                query.Append($"'{value}'");
            }
            else
            {
                query.Append($", '{value}') ");
            }
            return this;
        }

        // Método para UPDATE
        public static Sql Update(string table)
        {
            return new Sql($"UPDATE {table}");
        }

        public Sql Set(string columnValuePairs)
        {
            query.Append($" SET {columnValuePairs}");
            return this;
        }

        // Método para DELETE
        public static Sql DeleteFrom(string table)
        {
            return new Sql($"DELETE FROM {table}");
        }

        // Método para LIMIT
        public Sql Limit(int limit)
        {
            query.Append($" LIMIT {limit}");
            return this;
        }

        // Método para OFFSET
        public Sql Offset(int offset)
        {
            query.Append($" OFFSET {offset}");
            return this;
        }

        // Método para UNION
        public Sql Union(Sql otherQuery)
        {
            query.Append($" UNION {otherQuery}");
            return this;
        }

        // Método para EXISTS
        public Sql Exists(Sql subQuery)
        {
            query.Append($" EXISTS ({subQuery})");
            return this;
        }

        // Método para IN
        public Sql In(string values)
        {
            query.Append($" IN ({values})");
            return this;
        }

        // Método para BETWEEN
        public Sql Between(string value1, string value2)
        {
            query.Append($" BETWEEN {value1} AND {value2}");
            return this;
        }
        
        public Sql AddSql(Sql sql)
        {
            query.Append($" {sql.ToString()}");
            return this;
        }

        // Método para agregar directamente texto SQL
        public Sql Raw(string sql)
        {
            query.Append($" {sql}");
            return this;
        }

        // Sobrescribir ToString para devolver la consulta completa
        public override string ToString()
        {
            return query.ToString() + (endWithSemicolon ? ";" : "");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable)query)?.GetObjectData(info, context);
        }

        public bool Equals(Sql? other)
        {
            if (other == null)
            {
                return false;
            }
            return query.Equals(other.query);
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}


/*
SQL sql1 = SQL.Select("*").From("users").Where("age > 25");
SQL sql2 = SQL.Select("*").From("admins").Where("age > 25");

SQL unionSql = sql1.Union(sql2);
*/

/*
SQL subQuery = SQL.Select("*").From("orders").Where("user_id = users.id");
SQL sql = SQL.Select("*")
             .From("users")
             .Where("EXISTS (" + subQuery + ")");
*/

/*
SQL sql = SQL.Select("*")
             .From("users")
             .Where("id").In("1, 2, 3");
*/