namespace RRSSR
{
    internal class RssFeed
    {
        internal string Title { get; }
        internal string Description { get; }
        internal RssItem[] Items { get; }

        public RssFeed(string title, string description, RssItem[] items)
        {
            Title = string.IsNullOrEmpty(title) ? "<No title>" : title;
            Description = description;
            Items = items;
        }
    }
}
