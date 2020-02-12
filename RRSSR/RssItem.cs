namespace RRSSR
{
    class RssItem
    {
        public string Title { get; }
        public string Summary { get; }
        public string Link { get; }

        public RssItem(string title, string summary, string link)
        {
            Title = title;
            Summary = string.IsNullOrEmpty(summary) ? "<No summary>" : summary;
            Link = link;
        }
    }
}
