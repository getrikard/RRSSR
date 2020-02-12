using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RRSSR
{
    internal static class Settings
    {
        internal static List<string> Urls { get; private set; } = new List<string>();
        internal static int PaddingTop { get; private set; } = 2;
        internal static int PaddingLeft { get; private set; } = 4;
        internal static ConsoleColor FgColor { get; private set; } = ConsoleColor.Black;
        internal static ConsoleColor BgColor { get; private set; } = ConsoleColor.White;
        internal static int ItemsToGet { get; private set; } = 15;

        internal static int ReadFile(string filename)
        {
            var lineNumber = 0;

            if (!File.Exists(filename)) return lineNumber;

            var lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                lineNumber++;

                if (line.StartsWith("#")) continue;

                var settingLine = line.Split('=').ToList().Select(item => item.Trim()).ToArray();

                if (settingLine.Length <= 1) continue;

                try
                {
                    switch (settingLine[0])
                    {
                        case "rss_url":
                            Urls.Add(settingLine[1]);
                            break;
                        case "padding_top":
                            PaddingTop = Convert.ToInt32(settingLine[1]);
                            break;
                        case "padding_sides":
                            PaddingLeft = Convert.ToInt32(settingLine[1]);
                            break;
                        case "fg_color":
                            FgColor = StringToConsoleColor(settingLine[1]);
                            break;
                        case "bg_color":
                            BgColor = StringToConsoleColor(settingLine[1]);
                            break;
                        case "fetch_amount":
                            ItemsToGet = Convert.ToInt32(settingLine[1]);
                            break;
                    }
                }
                catch
                {
                    return lineNumber;
                }
            }

            return 0;
        }

        internal static void CreateFile(string filename)
        {
            if (File.Exists(filename))
            {
                Console.WriteLine("ERROR: Config file already exists, aborting.");
                return;
            }

            File.WriteAllText(filename, ToString());
        }

        internal static string ToString()
        {
            string txt = "";
            foreach (var url in Urls)
            {
                txt += $"rss_url = {url}\n";
            }
            txt += $"padding_top = {PaddingTop}\n";
            txt += $"padding_sides = {PaddingLeft}\n";
            txt += $"fg_color = {FgColor.ToString()}\n";
            txt += $"bg_color = {BgColor.ToString()}\n";
            txt += $"fetch_amount = {ItemsToGet}\n";
            return txt;
        }

        private static ConsoleColor StringToConsoleColor(string colorName)
        {
            switch (colorName)
            {
                case "black":
                    return ConsoleColor.Black;
                case "white":
                    return ConsoleColor.White;
                case "blue":
                    return ConsoleColor.Blue;
                case "red":
                    return ConsoleColor.Red;
                case "yellow":
                    return ConsoleColor.Yellow;
                case "magenta":
                    return ConsoleColor.Magenta;
                case "cyan":
                    return ConsoleColor.Cyan;
                case "green":
                    return ConsoleColor.Green;
                case "grey":
                case "gray":
                    return ConsoleColor.Gray;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
