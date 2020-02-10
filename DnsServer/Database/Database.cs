using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

using C = DnsServer.ConsoleIO.ConAPI;

namespace DnsServer.Database {
    class Database : IDatabase {

        

        // -------------------------------------------------------------------

        SqliteConnectionStringBuilder connectionStringBuilder;

        public Database(string file = "./application.db") {
            connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = file;
        }

        /// <summary>
        /// Check the Database master entities list to see if a type exists
        /// </summary>
        /// <param name="entityName">Name</param>
        /// <param name="type">Type</param>
        /// <param name="connection">Connection, if null a new one is created.</param>
        /// <returns>If the entity exists</returns>
        public bool DbEntityExists(string entityName, string type, SqliteConnection connection = null) {
            if (connection == null) connection = Connection(true);

            var sql = "SELECT name FROM sqlite_master WHERE type = @Type AND name = @Name;";
            using (var command = new SqliteCommand(sql, connection)) {
                command.Parameters.AddWithValue("@Name", entityName);
                command.Parameters.AddWithValue("@Type", type);

                using (var reader = command.ExecuteReader())
                    return reader.HasRows;
                //  return reader.GetValue(0) != null 
                //      ? (reader.GetValue(0) as string).ToLower() == tableName
                //      : false;
            }

            return false;
        }

        public void CreateEntities() {
            DatabaseSchema.CreateObjects(this);
        }

        public SqliteConnection Connection(bool openConnection = true) {
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            if (openConnection) connection.Open();
            return connection;
        }
    }
}
