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
        private readonly LibraryContext _context;

        public RequestBookService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<bool> RequestBook(int userId, int bookId, int libraryId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Load all data needed for the transaction
                var user = await _context.Users.FindAsync(userId);
                var library = await _context.Libraries.FindAsync(libraryId);
                var book = await _context.Books
                         .Include(b => b.Author)
                         .FirstOrDefaultAsync(b => b.BookId == bookId);

                if (user == null || library == null || book == null)
                {
                    throw new InvalidOperationException("Invalid request. User, Library, or Book not found.");
                }

                var copy = await _context.Copies
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

                _context.Orders.Add(order);
                copy.NumberOfCopies -= 1;

                await _context.SaveChangesAsync();

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
                    ReturnDate = order.ReturnDate
                };

                _context.OrderHistories.Add(orderHistory);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred.", ex);
            }
        }
    }
}