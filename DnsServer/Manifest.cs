using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace DnsServer {
    class Manifest {
        public static string ReadManifestData(string embeddedFileName) {
            return ReadManifestData<Manifest>(embeddedFileName);
        }

        public static string ReadManifestData<TSource>(string embeddedFileName) where TSource : class {
            var assembly = typeof(TSource).GetTypeInfo().Assembly;
            var resourceName = assembly.GetManifestResourceNames().First(s => s.EndsWith(embeddedFileName, StringComparison.CurrentCultureIgnoreCase));

            using (var stream = assembly.GetManifestResourceStream(resourceName)) {
                if (stream == null) 
                    throw new InvalidOperationException("Could not load manifest resource stream.");

                using (var reader = new StreamReader(stream)) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
