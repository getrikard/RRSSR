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
        static void Main(string[] args)
        {
            var items = GetRssItems("https://www.nrk.no/urix/toppsaker.rss", 20);
            var selected = 0;
            ConsoleKey keyPressed;
            Console.CursorVisible = false;
            bool doPrintMenu = true;

            while (true)
            {
                if (doPrintMenu) PrintMenu(items, selected);
                keyPressed = Console.ReadKey().Key;

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
                        PrintItem(items[selected]);
                        doPrintMenu = true;
                        break;
                    case ConsoleKey.Escape:
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
            int top = 1;
            int left = 4;
            Console.Clear();

            for (int i = 0; i < items.Length; i++)
            {
                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.SetCursorPosition(left, top);
                Console.Write(items[i].Title);
                top++;

                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        private static void PrintItem(RssItem item)
        {
            int top = 1;
            int left = 4;
            Console.Clear();
            PrintAtPosition(left, top, item.Summary);
            Console.ReadKey();
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
                .Select(item => new RssItem(item.Title.Text, item.Summary.Text))
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
