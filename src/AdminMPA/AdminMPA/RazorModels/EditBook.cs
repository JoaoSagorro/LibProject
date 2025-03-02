using Microsoft.AspNetCore.Mvc;

namespace AdminMPA.RazorModels
{
    [BindProperties]
    public class EditBook
    {
        public string Title { get; set; }
        public string Edition { get; set; }
        public int Year { get; set; }
        public string AuthorName { get; set; }
    }
}
