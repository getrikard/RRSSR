namespace RRSSR
{
    class RssItem
    {
        public string Title { get; }
        public string Summary { get; }

        public RssItem(string title, string summary)
        {
            Title = title;
            Summary = summary == "" ? null : summary;
        }
    }
}
