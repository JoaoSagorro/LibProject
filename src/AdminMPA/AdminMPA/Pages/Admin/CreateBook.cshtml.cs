using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AdminMPA.RazorModels;
using EFLibrary.Models;
using LibLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class CreateBookModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateBookModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public List<Item> Books { get; private set; }
        [BindProperty]
        public CreateBookDTO CreateBookDTO { get; set; }
        public List<Library> Libraries { get; set; }
        [BindProperty]
        public int targetLibrary { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            if(HttpContext.Session.GetString("User") != null)
            {
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    Libraries = LibLibraries.GetLibraries();
                    var apiUrl = $"https://www.googleapis.com/books/v1/volumes?q={SearchTerm}";
                    var response = await _httpClient.GetStringAsync(apiUrl);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var booksResponse = JsonSerializer.Deserialize<GoogleBooksResponse>(response, options);
                    Books = booksResponse?.Items ?? new List<Item>();
                }
                return Page();
            }
            return RedirectToPage("../Index");
       }

        public IActionResult OnPost(int quantity)
        {

            if(HttpContext.Session.GetString("User") != null)
            {
                try
                {
                var categoriesString = JsonSerializer.Deserialize<List<string>>(Request.Form["CreateBookDTO.Categories"]) ?? ["Unknown"];
                var subjectsList = categoriesString[0].Split(", ").ToList();
                var subjects = new List<Subject>();
                foreach( var sub in subjectsList)
                {
                    subjects.Add(new Subject { SubjectName = sub });
                }
                var img = LibCover.ConvertFileToImage(CreateBookDTO.CoverImage).Result;
                Author author = new Author { AuthorName = CreateBookDTO.AuthorName };
                if (!LibAuthor.AuthorExists(author.AuthorName)) LibAuthor.AddAuthor(author);
                Cover cover = new Cover { CoverImage = img};
                var yearString = CreateBookDTO.Year.Length >= 4 ? CreateBookDTO.Year.Substring(0, 4) : CreateBookDTO.Year;
                if (int.TryParse(yearString, out int year)) ;
                var newBook = new Book
                {
                    Title = CreateBookDTO.Title,
                    Quantity = quantity,
                    Edition = CreateBookDTO.Edition,
                    Year =year,
                    Cover = cover,
                    Subjects= subjects,
                    Author= author,

                };
                LibBooks.AddBook(newBook);
                var book = LibBooks.BookFinder(newBook.Title);
                LibCopies.CreateInitialCopies(book.BookId, targetLibrary, quantity);
                    return RedirectToPage("./TransferBook", new { libraryId = targetLibrary, bookId = book.BookId });
                }
                catch
                {
                    return Page();
                }
           }
            return RedirectToPage("../Index");
       }
    }
}
