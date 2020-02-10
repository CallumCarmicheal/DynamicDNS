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
            var webServer = setupWebServer(database, dnsServer);
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

        static Web.WebServer setupWebServer(Database.Database database, DnsServer.DnsServer dns) {
            var ws = new Web.WebServer();

            return ws;
        }
    }
}
