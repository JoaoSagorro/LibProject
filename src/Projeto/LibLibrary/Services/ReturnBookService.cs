using System;
using System.Linq;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace LibLibrary.Services
{
    public class ReturnBookService
    {
        private readonly LibraryContext _context;

        public ReturnBookService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<bool> ReturnBook(int userId, int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Book)
                    .Include(o => o.Library)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.User.UserId == userId);

                if (order == null)
                {
                    throw new InvalidOperationException("Order not found or user does not have this order.");
                }

                var copy = await _context.Copies
                    .FirstOrDefaultAsync(c => c.BookId == order.Book.BookId && c.LibraryId == order.Library.LibraryId);

                if (copy == null)
                {
                    throw new InvalidOperationException("Book copy not found in library.");
                }

                order.ReturnDate = DateTime.UtcNow;

                if(order.ReturnDate.HasValue && (order.ReturnDate.Value - order.OrderDate).Days > 15)
                {
                    LibUser.AddStrikeToUser(order.User);
                }
                copy.NumberOfCopies += 1;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred during the book return.", ex);
            }
        }
    }
}
