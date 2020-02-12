using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Nancy;
using Nancy.Responses;
using System.IO;

namespace DnsServer.Web.Modules {
    public class WebFront : NancyModule {
        private Database.IDatabase database;

        public WebFront(Database.IDatabase database) {
            this.database = database;
            Get("/", Index);
            Get("/view/{View}", Index);
        }

        private object Index(dynamic arg) {
            if (arg.View != null) {
                return View[arg.View];
            }

            return View["Index"];

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><body>");
            sb.Append("<h1>Domains with Auth Keys:</h1>");
            sb.Append("<table><head><tr><th>Domain</th></tr></head><tbody>");

            var assembly = typeof(WebFront).GetTypeInfo().Assembly;
            foreach (var x in assembly.GetManifestResourceNames()) {
                sb.Append($"<tr><td>{x}</td></tr>");
            }

            sb.Append("</tbody></table>");
            sb.Append("</body></html>");
            return HtmlResponse(sb.ToString());
        }

        private Response HtmlResponse(string html) {
            var response = new HtmlResponse();
            response.ContentType = "text/html; charset=utf-8";
            response.Contents = s => {
                var writer = new StreamWriter(s, Encoding.UTF8);
                writer.Write(html);
                writer.Flush();
            };
            return response;
        }
    }
}
