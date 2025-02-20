using System;
using System.Linq;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace LibLibrary.Services
{
    public class RequestBookService
    {
        //private readonly LibraryContext _context;

        //public RequestBookService(LibraryContext context)
        //{
        //    _context = context;
        //}

        private bool CanRequest(int numberOfCopies)
        {
            return numberOfCopies <= 4; 
        }

        public async Task<bool> RequestBook(int userId, int bookId, int libraryId, int numberOfCopies)
        {
            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    if (!CanRequest(numberOfCopies)) throw new Exception("Can't request more than 4 copies.");

                    var transaction = await context.Database.BeginTransactionAsync();
                    // Load all data needed for the transaction
                    var user = await context.Users.FindAsync(userId);
                    var library = await context.Libraries.FindAsync(libraryId);
                    var book = await context.Books
                             .Include(b => b.Author)
                             .FirstOrDefaultAsync(b => b.BookId == bookId);

                    if (user == null || library == null || book == null)
                    {
                        throw new InvalidOperationException("Invalid request. User, Library, or Book not found.");
                    }

                    var copy = await context.Copies
                        .FirstOrDefaultAsync(c => c.BookId == bookId && c.LibraryId == libraryId);

                    if (copy == null || copy.NumberOfCopies <= 1)
                    {
                        throw new InvalidOperationException("Book not available.");
                    }

                    // Create the order
                    var order = new Order
                    {
                        User = user,
                        Library = library,
                        Book = book,
                        OrderDate = DateTime.UtcNow,
                        ReturnDate = DateTime.UtcNow.AddDays(15)
                    };

                    context.Orders.Add(order);
                    copy.NumberOfCopies -= 1;

                    await context.SaveChangesAsync();

                    var orderHistory = new OrderHistory
                    {
                        UserName = user.FirstName,
                        BookName = book.Title,
                        BookYear = book.Year,
                        BookAuthor = book.Author.AuthorName,
                        BookEdition = book.Edition,
                        LibraryName = library.LibraryName,
                        OrderedCopies = 1,
                        OrderDate = order.OrderDate,
                        //ReturnDate = order.ReturnDate
                    };

                    context.OrderHistories.Add(orderHistory);

                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return true;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred.", ex);
            }
        }
    }
}