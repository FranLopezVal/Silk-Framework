namespace Silk.Database
{
    internal class DatabaseConnectionFactory
    {
        internal static IDatabaseConnection CreateInstance(string dbType, params object[] parameters)
        {
            foreach (var type in DatabasePlugin.pluginTypes)
            {
                if (type.Key == dbType.ToLower())
                {
                    return (IDatabaseConnection)Activator.CreateInstance(type.Value, parameters);
                }
            }
            throw new Exception("Database type not found");
        }
    }
}
