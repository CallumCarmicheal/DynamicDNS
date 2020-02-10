using System;
using System.Collections.Generic;
using System.Text;

namespace DnsServer.Web {
    public class NModule : Nancy.NancyModule {

        public NModule() {
            Get("/ddns/update", dynamicDns_UpdateHost);
        }

        private object dynamicDns_UpdateHost(dynamic arg) {
            
        }
    }
}
