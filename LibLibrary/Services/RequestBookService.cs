using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            using var transaction = _context.Database.BeginTransactionAsync();
            try
            {
                // Load all data needed for the transaction
                var data = await _context.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => new
                    {
                        User = u,
                        Library = _context.Libraries.FirstOrDefault(l => l.LibraryId == libraryId),
                        Book = _context.Books.FirstOrDefault(b => b.BookId == bookId),
                        State = _context.States.FirstOrDefault(s => s.StateName == "Requested"),
                    })
                    .FirstOrDefaultAsync();

                if (data is null || data.Library is null || data.Book is null || data.State is null)
                    throw new InvalidOperationException("Invalid request.");

                if (data.User.Suspended) throw new InvalidOperationException("User is suspended.");

                var copie = await _context.Copies.FirstOrDefaultAsync(c => c.BookId == bookId && c.LibraryId == libraryId);

                if (copie is null || copie.NumberOfCopies <= 1) throw new InvalidOperationException("Book not available.");

                var order = new Order
                {
                    User = data.User,
                    Library = data.Library,
                    Book = data.Book,
                    State = data.State,
                    OrderDate = DateTime.UtcNow,
                    ReturnDate = DateTime.UtcNow.AddDays(15)
                };

                _context.Orders.Add(order);

                copie.NumberOfCopies -= 1;

                await _context.SaveChangesAsync();
                await transaction.Result.CommitAsync();
                return true;
            }

            catch (Exception)
            {
                await transaction.Result.RollbackAsync();
                throw;
            }
        }
    }
}
