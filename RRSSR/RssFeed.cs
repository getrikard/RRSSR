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
            Description = string.IsNullOrEmpty(description) ? "<No title>" : description;
            Items = items;
        }
    }
}
