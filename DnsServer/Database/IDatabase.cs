using Microsoft.Data.Sqlite;

namespace DnsServer.Database {
    public interface IDatabase {
        SqliteConnection Connection(bool openConnection = true);
        void CreateEntities();
        bool DbEntityExists(string entityName, string type, SqliteConnection connection = null);
    }
}