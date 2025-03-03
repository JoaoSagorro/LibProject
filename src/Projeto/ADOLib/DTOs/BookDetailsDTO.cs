namespace ADOLib.DTOs
{
    public class BookDetailsDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Edition { get; set; }
        public int Year { get; set; }
        public string AuthorName { get; set; }
        public List<string> SubjectNames { get; set; }
    }
}