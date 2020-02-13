using System;
using System.Collections.Generic;
namespace RRSSR
{
    internal class RssFeed
    {
        internal string Title { get; }
        internal string Description { get; }
        internal RssItem[] Items { get; }

        public RssFeed(string title, string description, RssItem[] items)
        {
            Title = title;
            Description = description;
            Items = items;
        }
    }
}
