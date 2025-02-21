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
                    var state = context.States.First(a => a.StateId == 1);

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

                    // check if the remaining copies are >= 1:
                    int remaining = copy.NumberOfCopies - numberOfCopies;
                    if (remaining < 1) throw new Exception("Can't request the desired amount of copies");

                    // Create the order
                    var order = new Order
                    {
                        User = user,
                        Library = library,
                        Book = book,
                        State = state,
                        OrderDate = DateTime.UtcNow,
                        RequestedCopiesQTY = numberOfCopies,
                    };

                    context.Orders.Add(order);
                    copy.NumberOfCopies = remaining;

                    await context.SaveChangesAsync();

                    var orderHistory = new OrderHistory
                    {
                        UserName = user.FirstName,
                        BookName = book.Title,
                        BookYear = book.Year,
                        BookAuthor = book.Author.AuthorName,
                        BookEdition = book.Edition,
                        LibraryName = library.LibraryName,
                        OrderedCopies = numberOfCopies,
                        OrderDate = order.OrderDate
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