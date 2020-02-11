// Rikards RSS Reader

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.ServiceModel.Syndication;

namespace RRSSR
{
    class Program
    {
        private static int _paddingTop = 1;
        private static int _paddingLeft = 4;

        private static readonly ConsoleColor bgColor = ConsoleColor.Black;
        private static readonly ConsoleColor fgColor = ConsoleColor.Green;

        static void Main(string[] args)
        {
            var items = GetRssItems("https://www.nrk.no/urix/toppsaker.rss", 15);
            var selected = 0;
            ConsoleKey keyPressed;
            Console.CursorVisible = false;
            bool doPrintMenu = true;

            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
            Console.Clear();

            while (true)
            {
                if (doPrintMenu) PrintMenu(items, selected);
                keyPressed = Console.ReadKey(true).Key;
                var item = items[selected];

                switch (keyPressed)
                {
                    case ConsoleKey.DownArrow:
                        selected = selected + 1 < items.Length ? selected + 1 : items.Length - 1;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.UpArrow:
                        selected = selected - 1 >= 0 ? selected - 1 : 0;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.Home:
                        selected = 0;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.End:
                        selected = items.Length - 1;
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.Enter:
                        PrintItem(item);
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.O:
                        System.Diagnostics.Process.Start(item.Link);
                        doPrintMenu = false;
                        break;
                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                        Environment.Exit(0);
                        break;
                    default:
                        doPrintMenu = false;
                        break;
                }
            }
        }

        private static void PrintMenu(RssItem[] items, int selected)
        {
            int localTop = _paddingTop;
            Console.Clear();

            for (int i = 0; i < items.Length; i++)
            {
                if (i == selected)
                {
                    Console.BackgroundColor = fgColor;
                    Console.ForegroundColor = bgColor;
                }

                Console.SetCursorPosition(_paddingLeft, localTop);
                Console.Write(items[i].Title);
                localTop++;

                if (i == selected)
                {
                    Console.BackgroundColor = bgColor;
                    Console.ForegroundColor = fgColor;
                }
            }
        }

        private static void PrintItem(RssItem item)
        {
            Console.Clear();
            PrintAtPosition(_paddingLeft, _paddingTop, item.Summary);
            ConsoleKey keyPressed;
            while (true)
            {
                keyPressed = Console.ReadKey(true).Key;
                switch (keyPressed)
                {
                    case ConsoleKey.O:
                        System.Diagnostics.Process.Start(item.Link);
                        break;
                    default:
                        return;
                }
            }
        }

        private static int PrintAtPosition(int left, int top, string s)
        {
            int lines = 1;
            int windowWidth = Console.WindowWidth;
            int lineLength = windowWidth - left * 2;

            var buffer = new List<string>();
            if (s.Length > lineLength)
            {
                buffer.Add(s.Substring(0, lineLength));
                buffer.Add(s.Substring(lineLength));
                lines++;
            }
            else
            {
                buffer.Add(s);
            }

            foreach (var line in buffer)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(line);
                top++;
            }

            return lines;
        }

        private static RssItem[] GetRssItems(string url, int amount)
        {
            var feed = GetSyndicationFeed(url);
            return feed.Items
                .Select(item => new RssItem(item.Title.Text, item.Summary.Text, item.Links[0].Uri.ToString()))
                .Take(amount)
                .ToArray();
        }

        private static SyndicationFeed GetSyndicationFeed(string url)
        {
            using (var reader = XmlReader.Create(url))
            {
                return SyndicationFeed.Load(reader);
            }
        }
    }
}
