using Silk.SQL;
using System.Data;

namespace Silk.ORM
{
    /// <summary>
    /// The repository interface for the ORM, this is the main interface that is used to interact with the database.
    /// A repository is a collection of methods that are used to interact with the database.
    /// It like a table binding to a class, where the class is the model and the table is the repository.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>: IDisposable
    {
        T GetValue(object reference, string? targetKey);
        IEnumerable<T> GetValues(object reference, string? targetKey);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<object> CustomQuery(Sql sql);
        int CustomNonQuery(Sql sql);
        object CustomSingleQuery(Sql sql);
        object Join<T>(string table, string column, string joinColumn);
        IEnumerable<T> GetValues(int limit = 100);
    }

    /// <summary>
    /// Generic repository class that implements the IRepository interface.
    /// <typeparam name="dbContextType"> Is the type of the database connection, it can be any type that implements IDbConnection <see cref="IDbConnection"/></typeparam>
    /// <typeparam name="repositoryType"> Is the type of the model that the repository is binding to. model like table struct </typeparam>
    /// </summary>
    public class Repository<dbContextType,repositoryType>: IRepository<repositoryType> where dbContextType : IDbConnection, new()
    {
        private readonly DatabaseContext<dbContextType> _context;

        private readonly string _tableName;
        private readonly string _primaryKeyName;

        public string TableName => _tableName;

        public Repository(DatabaseContext<dbContextType> dbc)
        {
            _context = dbc;

            var tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(typeof(repositoryType), typeof(TableAttribute));            
            _tableName = tableAttribute != null ? tableAttribute.Name : typeof(repositoryType).Name;

            var primaryKeyProperty = typeof(repositoryType).GetProperties().FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute)));
            var autoIncrementProperty = typeof(repositoryType).GetProperties().FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(AutoIncrementAttribute)));
            if (primaryKeyProperty == null)
            {
                throw new Exception($"No primary key defined for {typeof(repositoryType).Name}");
            }
            _primaryKeyName = primaryKeyProperty.Name;
            CreateIfNotExists();
        }


        /// <summary>
        /// The Repository need a table to bind to, this method creates the table if it does not exist.
        /// </summary>
        private void CreateIfNotExists()
        {
            var properties = typeof(repositoryType).GetProperties().Select(prop =>
            {
                var primaryKeyProperty = typeof(repositoryType).GetProperties().FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute)));
                var autoIncrementProperty = typeof(repositoryType).GetProperties().FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(AutoIncrementAttribute)));               
                return $"{prop.Name} {SQLiteTypeConverter.GetDbType(prop.PropertyType)} " +
                $"{((primaryKeyProperty != null && prop.Name == primaryKeyProperty.Name) ? "PRIMARY KEY" : "")} " +
                $"{((primaryKeyProperty != null && prop.Name == primaryKeyProperty.Name) && 
                (autoIncrementProperty != null && prop.Name == autoIncrementProperty.Name) ? " AUTOINCREMENT" : "")}";
            });

            var columns = string.Join(",", properties);

            var _sql = Sql.CreateTable(_tableName, columns,true);
            _context.ExecuteNonQuery(_sql.ToString());
        }

        /// <summary>
        /// Delete an entity from the repository.
        /// </summary>
        /// <param name="entity"> The entity to delete from the repository </param>
        /// <exception cref="ArgumentNullException"> Throws an ArgumentNullException if the entity is null </exception>
        public void Delete(repositoryType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var _sql = Sql.DeleteFrom(_tableName).Where($"{_primaryKeyName} = @{_primaryKeyName}");
            _context.ExecuteNonQuery(_sql.ToString(), entity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"> The reference to the entity to get from the repository </param>
        /// <param name="targetKey"> The target key to use to get the entity from the repository </param>
        /// <returns> The entity from the repository </returns>
        /// <exception cref="ArgumentNullException"> Throws an ArgumentNullException if the reference or target is null </exception>
        public repositoryType GetValue(object reference, string targetKey)
        {
            var target = targetKey ?? _primaryKeyName;
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            var _sql = Sql.Select("*").From(_tableName).Where($"{target} = @_value");
            return _context.ExecuteSingleQuery<repositoryType>(_sql.ToString(), new { _value = reference });
        }

        /// <summary>
        /// Get a list of entities from the repository.
        /// </summary>
        /// <param name="reference"> The reference to the entity to get from the repository </param>
        /// <param name="targetKey"> The target key to use to get the entity from the repository </param>
        /// <returns> The list of entities from the repository </returns>
        /// <exception cref="ArgumentNullException"> Throws an ArgumentNullException if the reference or target is null </exception>
        public IEnumerable<repositoryType> GetValues(object reference, string? targetKey = null)
        {
            var target = targetKey ?? _primaryKeyName;
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            var _sql = Sql.Select("*").From(_tableName).Where($"{target} = @_value");
            return _context.ExecuteQuery<repositoryType>(_sql.ToString(), new { _value = reference });
        }

        /// <summary>
        /// Get all the entities from the repository.
        /// Is like a <c>'SELECT * FROM table'</c> query.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<repositoryType> GetValues(int limit = 100)
        {
            var _sql = Sql.Select("*").From(_tableName).Limit(limit);
            return _context.ExecuteQuery<repositoryType>(_sql.ToString());
        }


        /// <summary>
        /// Insert an entity into the repository.
        /// </summary>
        public void Insert(repositoryType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var properties = entity.GetType().GetProperties()
            .Where(prop => prop.GetValue(entity) != null && !Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute)))
            .Select(prop => prop.Name);

            var parameters = properties.Select(prop => $"@{prop}");

            var _sql = Sql.InsertInto(_tableName, $"{string.Join(",", properties)}", $"{string.Join(",", parameters)}");

            _context.ExecuteNonQuery(_sql.ToString(), entity);
        }


        /// <summary>
        /// Update an entity in the repository.
        /// </summary>
        /// <param name="entity"> The entity to update in the repository </param>
        /// <exception cref="ArgumentNullException"> Throws an ArgumentNullException if the entity is null </exception>
        public void Update(repositoryType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var properties = entity.GetType().GetProperties()
            .Where(prop => prop.GetValue(entity) != null && !Attribute.IsDefined(prop, typeof(PrimaryKeyAttribute)))
            .Select(prop => $"{prop.Name} = @{prop.Name}");

            var _sql = Sql.Update(_tableName).Set($"{string.Join(",", properties)}").Where($"{_primaryKeyName} = @{_primaryKeyName}");

            _context.ExecuteNonQuery(_sql.ToString(), entity);
        }

        /// <summary>
        /// Join two tables together.
        /// </summary>
        /// <typeparam name="T"> The type of the model to join with </typeparam>
        /// <param name="table"> The table to join with </param>
        /// <param name="column"> The column of the left table to join with </param>
        /// <param name="joinColumn"> The column of the right table to join with </param>
        /// <returns> The result of the join query </returns>
        /// <exception cref="Exception"> Throws an Exception if no property is found for the column </exception>
        public object Join<T>(string table, string column, string joinColumn)
        {
            ArgumentNullException.ThrowIfNull(table);
            ArgumentNullException.ThrowIfNull(column);
            ArgumentNullException.ThrowIfNull(joinColumn);

            var properties = typeof(repositoryType).GetProperties().Select(prop => prop.Name == column);

            if (!properties.Any())
            {
                throw new Exception($"No property found for {column}");
            }

            var _sql = Sql.Select("*").From(_tableName).Join(table,$"{_tableName}.{column} = {table}.{joinColumn}");
            return _context.ExecuteQueryAnonymous(_sql.ToString());
        }

        /// <summary>
        /// Execute a custom query on the repository.
        /// </summary>
        /// <param name="sql"> The sql query to execute </param>
        /// <returns> Returns an anonymous object </returns>
        public IEnumerable<object> CustomQuery(Sql sql)
        {
            return _context.ExecuteQueryAnonymous(sql.ToString());
        }
        
        public object CustomSingleQuery(Sql sql)
        {
            return _context.ExecuteSingleQueryAnonymous(sql.ToString());
        }

        /// <summary>
        /// Execute a custom non query on the repository.
        /// </summary>
        /// <param name="sql"> The sql query to execute </param>
        /// <returns></returns>
        public int CustomNonQuery(Sql sql)
        {
            return _context.ExecuteNonQuery(sql.ToString());
        }

        public void Dispose()
        {
            _context.Close();
        }
    }
}
