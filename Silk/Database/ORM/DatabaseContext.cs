using System.Data;
using System.Dynamic;

namespace Silk.ORM
{
    public abstract class DatabaseContext<T> where T : IDbConnection, new()
    {
        private readonly T _dbconnection;
        private readonly string _connectionString;

        public T DbConnection => _dbconnection;
        public string ConnectionString => _connectionString;

        public DatabaseContext(string connectionString)
        {
            _dbconnection =new T();
            _connectionString = connectionString;
        }

        public abstract T Use();
        public abstract void Close();

        public abstract object ExecuteSingleQueryAnonymous(string query, object parameters = null);
        public abstract IEnumerable<object> ExecuteQueryAnonymous(string query, object parameters = null);


        public abstract LT ExecuteSingleQuery<LT>(string query, object parameters = null);
        public abstract IEnumerable<LT> ExecuteQuery<LT>(string query, object parameters = null);

        public abstract int ExecuteNonQuery(string query, object parameters = null);
        protected void AddParameters(IDbCommand command, object parameters)
        {
            if (parameters == null) return;
            
            foreach (var property in parameters.GetType().GetProperties())
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = property.Name;
                parameter.Value = property.GetValue(parameters);
                command.Parameters.Add(parameter);
            }
             
        }
        protected LT MapObject<LT>(IDataRecord record)
        {
            var obj = Activator.CreateInstance<LT>();
            for (int i = 0; i < record.FieldCount; i++)
            {
                var property = typeof(LT).GetProperty(record.GetName(i));
                if (property != null && record[i] != DBNull.Value)
                {
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    var safeValue = Convert.ChangeType(record[i], propertyType);
                    property.SetValue(obj, safeValue, null);
                }
            }
            return obj;
        }
    
        protected dynamic DataReaderToObject(IDataReader reader)
        {
            var dataObject = new ExpandoObject();
            var dataObjectDict = (IDictionary<string, object>)dataObject;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == "COUNT(*)") // COUNT(*) is a special case
                {
                    return reader.GetValue(i) as long?;
                }
                string columnName = reader.GetName(i);
                object columnValue = reader.GetValue(i);

                dataObjectDict[columnName] = columnValue;
            }
            return dataObject;
        }
    }
}
