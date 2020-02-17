// Rikards RSS Reader

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace RRSSR
{
    class Program
    {
        private static void Main(string[] args)
        {
            var settingsFilename = "rrssr.conf";

            if (args.Length > 0 && args[0].Trim() == "create_config")
            {
                Settings.CreateFile(settingsFilename);
                Exit(0);
            }

            if (args.Length > 1) settingsFilename = args[1];
            var errorAtLineNumber = Settings.ReadFile(settingsFilename);

            // errorAtLineNumber = 0 betyr at alt er bra.
            if (errorAtLineNumber > 0)
            {
                Console.WriteLine($"ERROR: Could not parse settings file: \"{settingsFilename}\", at line {errorAtLineNumber}.");
                return;
            }

            SetupConsole();

            // 
            if (Settings.Urls.Count == 0)
            {
                if (args.Length < 1)
                {
                    Console.SetCursorPosition(Settings.PaddingLeft, Settings.PaddingTop);
                    Console.CursorVisible = true;
                    Console.Write("RSS URL > ");
                    var url = Console.ReadLine();
                    Settings.Urls.Add(url);
                    Console.CursorVisible = false;

                }

                foreach (var arg in args)
                {
                    Settings.Urls.Add(arg);
                }
            }

            // Setup
            var selectedFeed = 0;
            RssFeed feed = null;
            try
            {
                feed = GetRssItems(Settings.Urls[selectedFeed], Settings.ItemsToGet);
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine($"ERROR: Could not get RSS feed from URL: {Settings.Urls[selectedFeed]}");
                Exit(1);
            }

            var selected = 0;
            ConsoleKey keyPressed;
            Console.CursorVisible = false;
            RssItem item;

            int refreshFrom = 0;
            int refreshTo = feed.Items.Length;
            bool doClearMenu = true;
            bool doPrintMenu = true;

            // Main loop
            while (true)
            {
                if (doPrintMenu)
                {
                    PrintMenu(feed, selected, refreshFrom, refreshTo, doClearMenu);
                }
                keyPressed = Console.ReadKey(true).Key;
                item = feed.Items[selected];

                switch (keyPressed)
                {
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.PageDown:
                    case ConsoleKey.J:
                        refreshFrom = selected;
                        selected = selected + 1 < feed.Items.Length ? selected + 1 : feed.Items.Length - 1;
                        refreshTo = selected + 1;
                        doClearMenu = false;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.PageUp:
                    case ConsoleKey.K:
                        refreshTo = selected + 1;
                        selected = selected - 1 >= 0 ? selected - 1 : 0;
                        refreshFrom = selected;
                        doClearMenu = false;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.Home:
                        refreshFrom = 0;
                        refreshTo = feed.Items.Length;
                        selected = 0;
                        doClearMenu = false;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.End:
                        refreshFrom = 0;
                        refreshTo = feed.Items.Length;
                        selected = feed.Items.Length - 1;
                        doClearMenu = false;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.RightArrow:
                        PrintItem(item);
                        refreshFrom = 0;
                        refreshTo = feed.Items.Length;
                        doClearMenu = true;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.O:
                        System.Diagnostics.Process.Start(item.Link);
                        doPrintMenu = false;
                        break;
                    case ConsoleKey.R:
                        if (Settings.ReadFile(settingsFilename) > 0) break;
                        feed = GetRssItems(Settings.Urls[selectedFeed], Settings.ItemsToGet);
                        refreshFrom = 0;
                        refreshTo = feed.Items.Length;
                        doClearMenu = true;
                        doPrintMenu = true;
                        Console.CursorVisible = false;
                        break;
                    case ConsoleKey.OemPeriod:
                        selectedFeed = (selectedFeed + 1) % Settings.Urls.Count;
                        feed = GetRssItems(Settings.Urls[selectedFeed], Settings.ItemsToGet);
                        refreshFrom = 0;
                        refreshTo = feed.Items.Length;
                        doClearMenu = true;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.OemComma:
                        selectedFeed = (selectedFeed + Settings.Urls.Count - 1) % Settings.Urls.Count;
                        feed = GetRssItems(Settings.Urls[selectedFeed], Settings.ItemsToGet);
                        refreshFrom = 0;
                        refreshTo = feed.Items.Length;
                        doClearMenu = true;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                        Exit(0);
                        break;
                    default:
                        doPrintMenu = false;
                        break;
                }
            }
        }

        private static void SetupConsole()
        {
            Console.Title = "RRSSR";
            Console.BackgroundColor = Settings.BgColor;
            Console.ForegroundColor = Settings.FgColor;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Clear();
        }

        private static void Usage()
        {
            Console.WriteLine("Usage:\n");
            Console.WriteLine("    RRSSR.exe <rss_url>");
            Console.WriteLine("    RRSSR.exe create_config");
        }

        private static void Exit(int exitCode)
        {
            Console.ResetColor();
            Console.Clear();
            if (exitCode != 0)
            {
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            Environment.Exit(exitCode);
        }

        private static void PrintMenu(RssFeed feed, int selected, int from = 0, int to = Int32.MaxValue, bool doClear = true)
        {
            RssItem[] items = feed.Items;
            int localTop = Settings.PaddingTop + from + 1;
            if (doClear) Console.Clear();

            var headerText = feed.Title;
            localTop += PrintAtPosition(Settings.PaddingLeft, Settings.PaddingTop, headerText.Trim());

            if (!string.IsNullOrEmpty(feed.Description))
            {
                localTop += PrintAtPosition(Settings.PaddingLeft, Settings.PaddingTop + 1, feed.Description.Trim());
            }


            for (int i = from; i < items.Length && i < to; i++)
            {
                if (i == selected)
                {
                    Console.BackgroundColor = Settings.FgColor;
                    Console.ForegroundColor = Settings.BgColor;
                }

                localTop += PrintAtPosition(Settings.PaddingLeft, localTop, items[i].Title);

                if (i == selected)
                {
                    Console.BackgroundColor = Settings.BgColor;
                    Console.ForegroundColor = Settings.FgColor;
                }
            }
        }

        private static void PrintItem(RssItem item)
        {
            Console.Clear();
            PrintAtPosition(Settings.PaddingLeft, Settings.PaddingTop, item.Summary);
            ConsoleKey keyPressed;
            while (true)
            {
                keyPressed = Console.ReadKey(true).Key;
                switch (keyPressed)
                {
                    case ConsoleKey.O:
                        System.Diagnostics.Process.Start(item.Link);
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                    case ConsoleKey.Backspace:
                    case ConsoleKey.BrowserBack:
                        return;
                }
            }
        }

        private static int PrintAtPosition(int left, int top, string s)
        {
            var linesUsed = 0;
            var windowWidth = Console.WindowWidth;
            var lineLength = windowWidth - left * 2;
            var textLines = s.Split('\n');

            var buffer = new List<string>();

            foreach (var textLine in textLines)
            {
                SplitStringOnLinelength(textLine + "\n", lineLength, ref buffer);
            }

            foreach (var line in buffer)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(line);
                if (line[line.Length - 1] == '\n') top++;
                top++;
                linesUsed++;
            }

            return linesUsed;
        }

        // Stjålet fra StackOverflow
        private static void SplitStringOnLinelength(string s, int lineLength, ref List<string> buffer)
        {
            var offset = 0;
            while (offset < s.Length)
            {
                int size = Math.Min(lineLength, s.Length - offset);
                buffer.Add(s.Substring(offset, size));
                offset += size;
            }
        }

        private static RssFeed GetRssItems(string url, int amount)
        {
            Console.Clear();
            PrintAtPosition(Settings.PaddingLeft, Settings.PaddingTop,
                "Henter RSS fra: " + url + " ...");

            SyndicationFeed feed;
            using (var reader = XmlReader.Create(url))
            {
                feed = SyndicationFeed.Load(reader);
            }

            var items = feed.Items
                .Select(item =>
                {
                    string title = null;
                    if (item.Title != null) title = item.Title.Text.Trim();

                    string summary = null;
                    if (item.Summary != null) summary = item.Summary.Text.Trim();

                    string link = null;
                    if (item.Links.Count != 0) link = item.Links[0].Uri.ToString();
                    return new RssItem(title, summary, link);
                })
                .Take(amount)
                .ToArray();

            var feedTitle = feed.Title != null ? feed.Title.Text : "";
            var feedDescription = feed.Description != null ? feed.Description.Text : "";

            return new RssFeed(feedTitle, feedDescription, items);
        }
    }
}
