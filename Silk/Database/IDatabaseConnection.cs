using Silk.SQL;

namespace Silk.Database
{
    public interface IDatabaseConnection
    {
        void Open();
        T Open<T>() where T : new();
        void Close();
        void ExecuteQuery(string query);
        void ExecuteQuery(Sql sQL);
        T ExecuteQuery<T>(string query) where T : new();
        T ExecuteQuery<T>(Sql query) where T : new();
    }
}
