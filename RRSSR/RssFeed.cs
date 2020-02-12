using System;
using System.Collections.Generic;
namespace RRSSR
{
    internal class RssFeed
    {
        internal string Title { get; }
        internal RssItem[] Items { get; }

        public RssFeed(string title, RssItem[] items)
        {
            Title = title;
            Items = items;
        }
    }
}
