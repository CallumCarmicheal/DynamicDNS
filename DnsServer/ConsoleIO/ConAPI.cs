using System;
using System.Collections.Generic;
using System.Text;

namespace DnsServer.ConsoleIO {
    class ConAPI {
        // https://wiki.ess3.net/mc/
        // http://minecraft.tools/en/color-code.php
        public const string Reset = "§r";
        public const string DarkRed = "§4";
        public const string Red = "§c";
        public const string Gold = "§6";
        public const string Yellow = "§e";
        public const string DarkGreen = "§2";
        public const string Green = "§a";
        public const string Aqua = "§b";
        public const string DarkAqua = "§3";
        public const string DarkBlue = "§1";
        public const string Blue = "§9";
        public const string LightPurple = "§d";
        public const string DarkPurple = "§5";
        public const string White = "§f";
        public const string Grey = "§7";
        public const string DarkGrey = "§8";
        public const string Black = "§0";

        public static void Write(string text, bool showDate = false, string category = "") {
            printText(text, showDate, category);
        }

        public static void WriteLine() {
            printTextLine("\n", false, "");
        }

        public static void WriteLine(string text, bool showDate = false, string category = "") {
            printTextLine(text, showDate, category);
        }

        public static string generateCategory(string c) {
            if (!c.IsNullOrWhiteSpace())
                c = ("§r§b" + c.ToUpper() + "§r ").PadRight(20);
            return c;
        }

        public static void printText(string text, bool showDate = false, string category = "") {
            text = " " + text;
            category = generateCategory(category);

            ConsoleIO.WriteFormated((showDate ? getTimeStamp() : "") + category + text);
        }

        public static void printTextLine(string text, bool showDate = false, string category = "") {
            text = " " + text;
            category = generateCategory(category);

            ConsoleIO.WriteLineFormatted((showDate ? getTimeStamp() : "") + category + text);
        }

        public static string getTimeStamp() { return "§c" + DateTime.Now.ToString("HH:mm:ss") + "§r "; }
    }
}
