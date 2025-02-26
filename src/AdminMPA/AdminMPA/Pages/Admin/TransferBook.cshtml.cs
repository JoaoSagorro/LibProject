using EFLibrary.Models;
using EFLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibLibrary.Services;
using EFLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class TransferBookModel : PageModel
    {
         [BindProperty]
        public Copie  Copy { get; set; }
        public List<Copie> Copies { get; set; }
        public List<Library> Libraries { get; set; }
        public IActionResult OnGet(int libraryId, int bookId)
        {
            if (HttpContext.Session.GetString("User") != null){
                Copy = LibCopies.GetCopy(bookId,libraryId);
                Copies = LibBooks.GetCopies(Copy.Book);
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
                using var _context = new LibraryContext();
                // Retrieve the current copy
                var currentCopy = _context.Copies.FirstOrDefault(c => c.BookId == Copy.BookId && c.LibraryId == Copy.LibraryId);
                if (currentCopy == null)
                {
                    // Handle the case where the current copy is not found
                    ModelState.AddModelError(string.Empty, "The specified copy was not found.");
                    return Page();
                }

                // Retrieve the target library copy
                var targetCopy = _context.Copies.FirstOrDefault(c => c.BookId == Copy.BookId && c.LibraryId == targetLibraryId);
                if (targetCopy == null)
                {
                    var book = _context.Books.FirstOrDefault(b => b.BookId == Copy.BookId);
                    var targetLib = _context.Libraries.FirstOrDefault(l => l.LibraryId == targetLibraryId);
                    var newCopie = new Copie { Book = book, BookId = book.BookId, Library = targetLib, LibraryId = targetLib.LibraryId, NumberOfCopies = quantity };
                    currentCopy.NumberOfCopies -= quantity;
                    _context.Copies.Add(newCopie);
                    _context.Update(currentCopy);
                    _context.SaveChanges();
                    //LibCopies.UpdateNumberOfCopies(newCopie);
                    // Handle the case where the target copy is not found
                    //ModelState.AddModelError(string.Empty, "The target library copy was not found.");
                    //return Page();
                    return RedirectToPage("./EditBook", new { id = Copy.BookId }); // Redirect to a success page or another appropriate page
                }

                // Check if the quantity to transfer is valid
                if (quantity <= 0 || quantity >= currentCopy.NumberOfCopies)
                {
                    ModelState.AddModelError(string.Empty, "Invalid quantity specified.");
                    return Page();
                }

                // Update the number of copies
                currentCopy.NumberOfCopies -= quantity;
                targetCopy.NumberOfCopies += quantity;

                _context.Update(currentCopy);
                _context.Update(targetCopy);
                _context.SaveChanges();

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
