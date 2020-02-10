using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using C = DnsServer.ConsoleIO.ConAPI;

namespace DnsServer.DnsServer {
    class DnsServer {
        ARSoft.Tools.Net.Dns.DnsClient dnsFallback = null;
        ARSoft.Tools.Net.Dns.DnsServer dnsServer = null;
        Database.Database  db;

        public DnsServer(Database.Database db, int udpPort = 53, int tcpPort = 53, List<IPAddress> fallbackIpAddress = null) {
            this.db = db;

            if (fallbackIpAddress != null) {
                dnsFallback = new DnsClient(fallbackIpAddress, 2000);
            }

            dnsServer = new ARSoft.Tools.Net.Dns.DnsServer(udpPort, tcpPort);
            dnsServer.QueryReceived += Server_QueryReceived;
        }

        public void Start() => dnsServer.Start();

        private async Task Server_QueryReceived(object sender, ARSoft.Tools.Net.Dns.QueryReceivedEventArgs e) {
            bool foundQuery = false;

            DnsMessage query = e.Query as DnsMessage;
            if (query == null) return;

            DnsMessage response = query.CreateResponseInstance();

            try {
                C.WriteLine($"DnsServer: Questions: {query.Questions.Count}", true, "DNS");

                for (int idx = 0; idx < query.Questions.Count; idx++) {
                    var question = query.Questions[idx];

                    using (var connection = db.Connection()) {
                        using (var command = new SqliteCommand(
                                "SELECT * FROM DnsEntries WHERE RecordType = @RecordType AND RecordClass = @RecordClass AND DomainName = @DomainName;", connection)) {
                            command.Parameters.AddWithValue("@RecordType",  question.RecordType.ToString());
                            command.Parameters.AddWithValue("@RecordClass", question.RecordClass.ToString());
                            command.Parameters.AddWithValue("@DomainName",  question.Name.ToString());

                            using (var reader = command.ExecuteReader()) {
                                C.WriteLine($"  Questions [{idx:000}] RES({reader.HasRows}): Type({question.RecordType}), Class({question.RecordClass}), {question.Name}", true, "DNS");

                                //Every new row will create a new dictionary that holds the columns
                                while (reader.Read()) {    
                                    try {
                                        var dnStr = reader["DomainName"] as string;
                                        var dn = DomainName.Parse(dnStr);
                                        if (dn == null) 
                                            continue;

                                        var ttl = (System.Int64)reader["TTL"];
                                        var rt = (RecordType)Enum.Parse(typeof(RecordType), reader["RecordType"] as string);
                                        var rc = (RecordClass)Enum.Parse(typeof(RecordClass), reader["RecordClass"] as string);

                                        if (rt == RecordType.A) {
                                            if (reader["Address"] == null) 
                                                continue;
                                            var addr = reader["Address"] as string;

                                            var aRec = new ARecord(dn, Convert.ToInt32(ttl), IPAddress.Parse(addr));
                                            response.AnswerRecords.Add(aRec);

                                            foundQuery = true;
                                        }
                                    } catch (Exception ex) {
                                        C.WriteLine($"  Questions [{idx:000}]: {C.Red}Exception: {ex.ToString()}", true, "DNS:ERROR");
                                    }
                                }
                            }
                        }
                    }
                }

                // Fallback
                if (dnsFallback != null && !foundQuery) {
                    DnsQuestion question = query.Questions[0];
                    var resolve = dnsFallback.Resolve(question.Name);

                    DnsMessage upstreamResponse = await dnsFallback.ResolveAsync(
                        question.Name, question.RecordType, question.RecordClass);

                    e.Response = upstreamResponse;
                    return;
                }

                response.ReturnCode = ReturnCode.NoError;
            } catch (Exception ex) {
                response.ReturnCode = ReturnCode.ServerFailure;
                C.WriteLine($"{C.Red}{ex.ToString()}", true, "DNS:ERROR");
            }

            // set the response
            e.Response = response;
        }
    }
}
