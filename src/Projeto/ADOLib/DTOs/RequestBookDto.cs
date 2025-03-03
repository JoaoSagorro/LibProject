namespace ADOLib.DTOs
{
    public class RequestBookDto
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int LibraryId { get; set; }
        public int NumberOfCopies { get; set; }
    }
}