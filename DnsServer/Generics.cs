﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DnsServer {
    public static class Generics {
        public static bool IsNullOrWhiteSpace(this string value) {
            if (value == null) return true;
            return string.IsNullOrWhiteSpace(value.Trim());
        }

        public static string Fmt(this string format, params object[] args) {
            return string.Format(format, args);
        }
    }
}
