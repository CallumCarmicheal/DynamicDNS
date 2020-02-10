using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Text;
using C = DnsServer.ConsoleIO.ConAPI;

namespace DnsServer.Web {
    class WebServer {
        private NancyHost   host = null;
        private int         port = -1;

        public WebServer(Database.Database database, int port = 8099) {
            this.port = port;

            var hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;
            hostConfigs.RewriteLocalhost = false;

            var bootstrap = new Bootstrapper(database);

            host = new NancyHost(new Uri($"http://localhost:{port}"), bootstrap, hostConfigs);
        }

        public void Start() {
            host.Start();

            C.WriteLine($"Web Server running on http://0.0.0.0.{port}", true, "Web");
        }
    }
}
