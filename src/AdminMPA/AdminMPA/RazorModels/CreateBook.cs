namespace AdminMPA.RazorModels
{
    public class CreateBookDTO
    {
        public string Title { get; set; }
        public string Edition { get; set; }
        public string Year { get; set; }
        public int Quantity { get; set; }
        public string AuthorName { get; set; }
        public List<string> Categories { get; set; }
        public string CoverImage { get; set; }
    }

    public class GoogleBooksResponse
    {
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public VolumeInfo VolumeInfo { get; set; }
    }

    public class VolumeInfo
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Description { get; set; }
        public ImageLinks ImageLinks { get; set; }
        public string PublishedDate { get; set; }
        public string Publisher { get; set; }
        public List<string> Categories { get; set; }
    }

    public class ImageLinks
    {
        public string SmallThumbnail { get; set; }
        public string Thumbnail { get; set; }
    }
}
