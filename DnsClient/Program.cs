using System;
using System.Net;

namespace DnsClient {
    class Program {
        static void Main(string[] args) {
            var ip = IPAddress.Parse("127.0.0.1");
            ARSoft.Tools.Net.Dns.DnsClient client = new ARSoft.Tools.Net.Dns.DnsClient(ip, 8000);

            Console.WriteLine("Type q to quit!");
            string input = "";
            while (input.ToLower() != "q") {
                try {
                    Console.Write("Resolv> ");
                    input = Console.ReadLine();
                    if (input.ToLower() == "q") continue;

                    if (input.ToLower() == "$cls$") {
                        Console.Clear();
                        continue;
                    }

                    var domainName = ARSoft.Tools.Net.DomainName.Parse(input);
                    var response = client.Resolve(domainName);
                    if (response == null) {
                        Console.WriteLine("   ERROR: DNS Server failed to respond to request.");
                        continue;
                    }

                    Console.WriteLine($"  Return code: {response.ReturnCode}");

                    if (response.AnswerRecords.Count > 0) {
                        Console.WriteLine($"  Found {response.AnswerRecords.Count:000} ");

                        foreach (var x in response.AnswerRecords) {
                            if (x.RecordType == ARSoft.Tools.Net.Dns.RecordType.A) {
                                var recordA = x as ARSoft.Tools.Net.Dns.ARecord;
                                if (recordA == null) {
                                    Console.WriteLine($"    {x.RecordType}, {x.RecordClass}, TTL: {x.TimeToLive} | {x.Name} | Failed to parse A record.");
                                } else {
                                    Console.WriteLine($"    {x.RecordType}, {x.RecordClass}, TTL: {x.TimeToLive} | {x.Name} | {recordA.Address}");
                                }
                            } else {
                                Console.WriteLine($"    {x.RecordType}, {x.RecordClass}, TTL: {x.TimeToLive} | {x.Name}");
                            }
                        }
                    } else {
                        Console.WriteLine($" No answers found");
                    }
                } catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
                
            }
        }
    }
}
