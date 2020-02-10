using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

using C = DnsServer.ConsoleIO.ConAPI;

namespace DnsServer.Database {
    class Database {

        const string SQL_TABLE_CREATE_DnsEntries = @"
            CREATE TABLE DnsEntries (
	            Id	            INTEGER 	PRIMARY KEY	AUTOINCREMENT,
	            Type	        TEXT                    DEFAULT 'DNS',
	            RecordType	    TEXT 		NOT NULL,
	            RecordClass	    TEXT 		NOT NULL 	DEFAULT 'INET',
	            DomainName	    TEXT		NOT NULL,
	            TTL	            INTEGER     NOT NULL    DEFAULT 43200,
	            Address	        TEXT,
	            Txt	            TEXT,
	            DateAdded	    TIMESTAMP	NOT NULL	DEFAULT CURRENT_TIMESTAMP,
	            DateLastUpdated	TIMESTAMP	NOT NULL	DEFAULT CURRENT_TIMESTAMP
            )
        ";

        const string SQL_TRIGGER_CREATE_DnsEntries_UpdateDateLastTime = @"  
            CREATE TRIGGER DnsEntries_UpdateDateLastTime
	        AFTER UPDATE ON DnsEntries 
		        FOR EACH ROW WHEN NEW.DateLastUpdated <= OLD.DateLastUpdated 
	        BEGIN
		        UPDATE DnsEntries 
			        SET DateLastUpdated = CURRENT_TIMESTAMP
			        WHERE Id = old.Id;
	        END;
        ";

        // -------------------------------------------------------------------

        SqliteConnectionStringBuilder connectionStringBuilder; 

        public Database(string file = "./application.db") {
            connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = file;
        }

        public bool DbEntityExists(string tableName, string type, SqliteConnection connection = null) {
            if (connection == null) connection = Connection(true);

            var sql = "SELECT name FROM sqlite_master WHERE type = @Type AND name = @Name;";

            using (var command = new SqliteCommand(sql, connection)) {
                command.Parameters.AddWithValue("@Name", tableName);
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
            var connection = Connection(true);

            if (!DbEntityExists("DnsEntries", "table")) {
                using (SqliteCommand command = new SqliteCommand(SQL_TABLE_CREATE_DnsEntries, connection)) {
                    int rows;
                    if ((rows = command.ExecuteNonQuery()) > 0) {
                        C.WriteLine($"{C.Yellow}Added Table: DnsEntries", true, "Database");
                    }
                }
            }

            if (!DbEntityExists("DnsEntries_UpdateDateLastTime", "trigger")) {
                using (SqliteCommand command = new SqliteCommand(SQL_TRIGGER_CREATE_DnsEntries_UpdateDateLastTime, connection)) {
                    int rows;
                    if ((rows = command.ExecuteNonQuery()) > 0) {
                        C.WriteLine($"{C.Yellow}Added Trigger: DnsEntries_UpdateDateLastTime", true, "Database");
                    }
                }
            }
        }

        public SqliteConnection Connection(bool openConnection = true) {
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            if (openConnection) connection.Open();
            return connection;
        }
    }
}
