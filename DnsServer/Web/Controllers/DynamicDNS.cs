using System;
using System.Collections.Generic;
using System.Text;

namespace DnsServer.Web.Modules {
    public class DynamicDNS : Nancy.NancyModule {
        private Database.IDatabase database;

        public DynamicDNS(Database.IDatabase database) {
            this.database = database;

            Get("/ddns/update", UpdateHost);
        }

        private object UpdateHost(dynamic parameters) {
            return "Database: " + database.DbEntityExists("DnsEntries", "table");
        }
    }
}
