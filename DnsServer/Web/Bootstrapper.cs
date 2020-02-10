using Nancy;
using Nancy.Configuration;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnsServer.Web {
    class Bootstrapper : DefaultNancyBootstrapper {
        Database.Database db;

        public Bootstrapper(Database.Database database) {
            db = database;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            base.ConfigureApplicationContainer(container);
            container.Register<Database.IDatabase>(db);

            //container.Register( (ctr, param) => new Modules.DynamicDNS((Database.IDatabase)db) );
        }

        public override void Configure(INancyEnvironment environment) {
            base.Configure(environment);
        }
    }
}
