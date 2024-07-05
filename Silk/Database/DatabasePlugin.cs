using System.Reflection;

namespace Silk.Database
{

    /// <summary>
    /// Used to mark a class as a database plugin.
    /// This class will be loaded by the DatabasePlugin class.
    /// <para>DatabaseType: The type of the database, this is used to identify the database.</para>
    /// <para/>
    /// This attribute only works on classes that implement IDatabaseConnection, and use in external assemblies. (dll) plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DatabasePluginAttribute : Attribute
    {
        public string DatabaseType { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }

        public DatabasePluginAttribute(string databaseType, string? connectionString, string? databaseName = null)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }
    }

    public class DatabasePlugin
    {
        internal static Dictionary<string, Type> pluginTypes = new Dictionary<string, Type>();

        internal static (string?,IDatabaseConnection?) databaseConnection = (null,null);

        public static IDatabaseConnection GetDatabaseConnection(string dbType, params object[] parameters)
        {
            if (databaseConnection.Item1 == dbType)
            {
                return databaseConnection.Item2;
            }
            var connection = DatabaseConnectionFactory.CreateInstance(dbType, parameters);
            databaseConnection = (dbType, connection);
            return connection;
        }

        internal static void LoadPlugins(string path)
        {
            pluginTypes.Clear();
            pluginTypes.Add("sqlite", typeof(SqLiteImplementation)); // Add the default implementation

            if (!System.IO.Directory.Exists(path))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(path,"*.dll"))
            {
                var assembly = Assembly.LoadFrom(file);
                foreach (var type in assembly.GetTypes())
                {
                    var attribute = type.GetCustomAttribute<DatabasePluginAttribute>();
                    if (attribute != null)
                    {
                        pluginTypes[attribute.DatabaseType.ToLower()] = type;
                    }
                }
            }
        }
    }
}
