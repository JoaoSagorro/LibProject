using EFLibrary.Models;
using EFLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class TransferBookModel : PageModel
    {
         [BindProperty]
        public Copie  Copy { get; set; }
        public List<Copie> Copies { get; set; }
        public List<Library> Libraries { get; set; }
        public string ImgSrc { get; set; }
         
        public IActionResult OnGet(int libraryId, int bookId)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                Copy = LibCopies.GetCopy(bookId,libraryId);
                Copies = LibBooks.GetCopies(Copy.Book);
                Cover cover = LibCover.GetCoverById(bookId);
                if(cover is not null)
                {
                    var base64 = Convert.ToBase64String(cover.CoverImage);
                    ImgSrc = String.Format("data:image/gif;base64,{0}", base64);
                }

                Libraries = LibLibraries.GetLibraries();
                int libIndex = Libraries.FindIndex(l => l.LibraryId == Copy.LibraryId);
                if(libIndex >= 0)
                {
                    Libraries.RemoveAt(libIndex);
                }
                int index = Copies.FindIndex(c => c.BookId == bookId && c.LibraryId == libraryId);
                if (index >= 0)
                {
                    Copies.RemoveAt(index);
                }
                return Page();
            }
            return RedirectToPage("../Index");
        }

        public IActionResult OnPost(int targetLibraryId, int quantity)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                // Retrieve the current copy
                var currentCopy = LibCopies.GetCopy(Copy.BookId, Copy.LibraryId);
                if (currentCopy == null)
                {
                    // Handle the case where the current copy is not found
                    ModelState.AddModelError(string.Empty, "The specified copy was not found.");
                    return Page();
                }

                // Check if the quantity to transfer is valid
                if (quantity <= 0 || quantity >= currentCopy.NumberOfCopies)
                {
                    ModelState.AddModelError(string.Empty, "Invalid quantity specified.");
                    return Page();
                }

                TransferService.TransferCopies(currentCopy, targetLibraryId, quantity);

                return RedirectToPage("./EditBook", new {id = Copy.BookId}); // Redirect to a success page or another appropriate page
            }
            return RedirectToPage("../Index");
        }

        public IActionResult OnPostUpdateCopyValue(int buyQuantity)
        {
            if(HttpContext.Session.GetString("User") != null)
            {

                Copy = LibCopies.GetCopy(Copy.BookId, Copy.LibraryId);
                Copy.NumberOfCopies = buyQuantity;
                LibCopies.UpdateNumberOfCopies(Copy);
                return RedirectToPage("./EditBook", new { id = Copy.BookId });
            }
            return RedirectToPage("../Index");
        }
    }
}
