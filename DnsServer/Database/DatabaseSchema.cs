using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

using C = DnsServer.ConsoleIO.ConAPI;

namespace DnsServer.Database {
    class DatabaseSchema {
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

		const string SQL_TABLE_CREATE_DynamicDnsAuthKeys = @"
			CREATE TABLE DynamicDnsAuthKeys (
				Id	            INTEGER 	PRIMARY KEY	AUTOINCREMENT,
				Type			TEXT		NOT NULL 	DEFAULT 'Default',
				RecordType		TEXT		NOT NULL,
				RecordClass		TEXT		NOT NULL,
				DomainName		TEXT		NOT NULL,
	
				AuthKey			TEXT		NOT NULL,
				DateLastUsed	TIMESTAMP	DEFAULT NULL,
	
				DateAdded	    TIMESTAMP	NOT NULL	DEFAULT CURRENT_TIMESTAMP,
				DateLastUpdated	TIMESTAMP	NOT NULL	DEFAULT CURRENT_TIMESTAMP
			)
		";

		const string SQLFMT_TRIGGER_CREATE_UpdateTimestampOnChange = @"  
            CREATE TRIGGER {0}_Update{2}_CurrentTimestamp
	        AFTER UPDATE ON DnsEntries 
		        FOR EACH ROW WHEN NEW.{2} <= OLD.{2} 
	        BEGIN
		        UPDATE DnsEntries 
			        SET DateLastUpdated = CURRENT_TIMESTAMP
			        WHERE {1} = old.{1};
	        END;
        ";

		public static void CreateObjects(Database database) {
			// Create database entities
			CreateTable(database, "DnsEntries",			SQL_TABLE_CREATE_DnsEntries);
			CreateTable(database, "DynamicDnsAuthKeys", SQL_TABLE_CREATE_DynamicDnsAuthKeys);

			CreateTimestampTrigger(database, "DnsEntries", "Id", "DateLastUpdated");
        }

		private static void CreateTable(Database database, string TableName, string Sql) {
			// Check if the table exists
			if (!database.DbEntityExists(TableName, "table")) {
				// Run the sql query to create it
				using (SqliteCommand command = new SqliteCommand(Sql, database.Connection())) {
					int rows;
					if ((rows = command.ExecuteNonQuery()) > 0) {
						C.WriteLine($"{C.Yellow}Created Table: {TableName}", true, "Database");
					}
				}
			}
		}

		private static void CreateTimestampTrigger(Database database, string Table, string IdColumn, string TimestampColumn) {
			var triggerName = $"{Table}_Update{TimestampColumn}_CurrentTimestamp";
			var sql = string.Format(SQLFMT_TRIGGER_CREATE_UpdateTimestampOnChange, Table, IdColumn, TimestampColumn);

			if (!database.DbEntityExists(triggerName, "trigger")) {
				AddEntity(database, sql, "Created Trigger: " + triggerName);
			}
		}

		private static void AddEntity(Database database, string sql, string debugMessage) {
			using (SqliteCommand command = new SqliteCommand(SQL_TABLE_CREATE_DnsEntries, database.Connection())) {
				int rows;
				if ((rows = command.ExecuteNonQuery()) > 0) {
					C.WriteLine($"{C.Yellow}{debugMessage}", true, "Database");
				}
			}
		}
    }
}
