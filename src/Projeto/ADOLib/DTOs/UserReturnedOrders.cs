namespace ADOLib.DTOs
{
    public class UserReturnedOrdersDTO
    {
        public int OrderId { get; set; }
        public string Title { get; set; } 
        public string AuthorName { get; set; } 
        public int? LibraryId { get; set; } 
        public string LibraryName { get; set; }
        public int RequestedCopiesQTY { get; set; } 
        public DateTime OrderDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string StateName { get; set; }
    }
}
