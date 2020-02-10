using DnsServer.ConsoleIO;
using System;
using System.Collections.Generic;
using System.Net;
using C = DnsServer.ConsoleIO.ConAPI;

namespace DnsServer {
    class Program {




        static void Main(string[] args) {
            C.WriteLine("Setting up local database...", true, "Bootstrap");
            var database = new Database.Database();
            database.CreateEntities();

            C.WriteLine("Starting dns server...", true, "Bootstrap");
            var dnsServer = setupDnsServer(database);
            dnsServer.Start();

            C.WriteLine("Starting web server...", true, "Bootstrap");
            var webServer = setupWebServer(database);
            if (webServer != null)
                webServer.Start();

            C.WriteLine("Press enter to stop!", true, "Bootstrap");
            Console.ReadLine();
        }


        static DnsServer.DnsServer setupDnsServer(Database.Database database) {
            return new DnsServer.DnsServer(
                database,
                fallbackIpAddress: new List<IPAddress>() {
                    IPAddress.Parse("172.16.10.1"),
                    IPAddress.Parse("172.16.11.1")
                });
        }

        static Web.WebServer setupWebServer(Database.Database database) {
            try {
                var ws = new Web.WebServer(database);
                return ws;
            } catch(Exception ex) {
                C.WriteLine($"{C.Red}Failed to start web server!", true, "Bootstrap:Web");
                C.WriteLine($"{C.Red}{ex.ToString()}", true, "Bootstrap:Web");
                return null;
            }
        }
    }
}
