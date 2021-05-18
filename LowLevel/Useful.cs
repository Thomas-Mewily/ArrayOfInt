using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace LowLevel
{
    public static class Useful
    {
        public static void Load() 
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            ResetConsoleColor();
        }

        public static void ResetConsoleColor(ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        public static string ToHexa(int i) => i.ToString("X4");
        public static string ToHexa(int i, int nbDigit) => i.ToString("X" + nbDigit);
        public static string ToBase10(int i) => (i < 0 ? "-" : " ") + Math.Abs(i).ToString("D10");

        public static string LengthCenter(this string s, int minLength) 
        {
            if (s.Length > minLength) { return s.Substring(0, minLength); }
            while (s.Length < minLength)
            {
                s = " " + s;
                if (s.Length >= minLength) { break; }
                s += " ";
            }
            return s;
        }
    }
}
