using EFLibrary;
using LibLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using System.Data;

namespace ADOLib
{
    public class ReturnBook
    {
        private readonly string _cnString;

        public ReturnBook()
        {
            _cnString= "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public void ReturnBook(int orderId)
        {
            using var connection = DB.Open(_cnString);
            var transaction = connection.BeginTransaction();

            try
            {
                string returnOrder = $"Update Orders Set ReturnDate = GETDATE() WHERE OrderId = {orderId}";
                string returnCopies = $@"UPDATE c
                                        SET c.NumberOfCopies = c.NumberOfCopies + o.RequestedCopiesQTY
                                        FROM Copie c
                                        INNER JOIN Orders o ON c.BookId = o.BookId AND c.LibraryId = o.LibraryId
                                        WHERE o.OrderId = {orderId};";
                string isOverdue = $"SELECT OrderDate FROM Orders o WHERE o.OrderId = {orderId};";
                DataTable overdue = DB.GetSQLRead(connection, isOverdue);
                DateTime date = Convert.ToDateTime(overdue.Rows[0]["OrderDate"]);
                if ((DateTime.UtcNow - date).Days > 15)
                {
                    var user = new Users();
                    int strikes = user.StrikeUser(orderId);
                    if (strikes > 3)
                    {
                        string suspendQuery = $@"UPDATE u
                                                SET u.Suspended=1, u.Active=0
                                                FROM Users u
                                                INNER JOIN Orders o ON o.UserId = u.UserId
                                                WHERE o.OrderId = {orderId};";
                        var userId = int.Parse(DB.GetSQLRead(connection,suspendQuery).Rows[0]["userId"].ToString());
                            
                    }
                }
            }
            catch(Exception e)
            {
                transaction.Rollback();
                throw new Exception($"Error returning book: {e}");
            }
            //    var order = await _context.Orders
            //        .Include(o => o.User)
            //        .Include(o => o.Book)
            //        .Include(o => o.Library)
            //        .FirstOrDefaultAsync(o => o.OrderId == orderId && o.User.UserId == userId);

            //    if (order == null)
            //    {
            //        throw new InvalidOperationException("Order not found or user does not have this order.");
            //    }

            //    var copy = await _context.Copies
            //        .FirstOrDefaultAsync(c => c.BookId == order.Book.BookId && c.LibraryId == order.Library.LibraryId);

            //    if (copy == null)
            //    {
            //        throw new InvalidOperationException("Book copy not found in library.");
            //    }

            //    order.ReturnDate = DateTime.UtcNow;

            //    if (order.ReturnDate.HasValue && (order.ReturnDate.Value - order.OrderDate).Days > 15)
            //    {
            //        LibUser.AddStrikeToUser(order.User);
            //    }
            //    copy.NumberOfCopies += 1;

            //    await _context.SaveChangesAsync();

            //    await transaction.CommitAsync();

            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    await transaction.RollbackAsync();
            //    Console.WriteLine($"Unexpected error: {ex.Message}");
            //    throw new InvalidOperationException("An unexpected error occurred during the book return.", ex);
            //}
        }
    }
}
