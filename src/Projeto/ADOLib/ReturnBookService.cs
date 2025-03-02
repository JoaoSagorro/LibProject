using EFLibrary;
using LibLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using System.Data;
using Microsoft.Data.SqlClient;
using ADOLib.Enums;

namespace ADOLib
{
    public class ReturnBookService
    {
        private readonly string CnString;

        public ReturnBookService()
        {
            //CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
            CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        public async void ReturnBookByOrderId(int orderId)
        {
            using var connection = DB.Open(CnString);
            using var transaction = connection.BeginTransaction();

            try
            {
                // Check if the order exists and if it's overdue
                string checkOrderQuery = "SELECT OrderDate FROM Orders WHERE OrderId = @OrderId";
                using var checkOrderCmd = new SqlCommand(checkOrderQuery, connection, transaction);
                checkOrderCmd.Parameters.AddWithValue("@OrderId", orderId);
                var orderDate = (DateTime)checkOrderCmd.ExecuteScalar();

                if ((DateTime.UtcNow - orderDate).Days > 15)
                {
                    var user = new Users();
                    int strikes = user.StrikeUser(orderId);

                    if (strikes > 3)
                    {
                        string suspendQuery = @"
                    UPDATE Users SET Suspended = 1, Active = 0
                    WHERE UserId IN (SELECT UserId FROM Orders WHERE OrderId = @OrderId)";
                        using var suspendCmd = new SqlCommand(suspendQuery, connection, transaction);
                        suspendCmd.Parameters.AddWithValue("@OrderId", orderId);
                        suspendCmd.ExecuteNonQuery();
                    }
                }

                // Update the order return date
                string returnOrderQuery = $"UPDATE Orders SET ReturnDate = GETDATE(), StateId = {(int)StatesEnum.Devolvido} WHERE OrderId = @OrderId";
                using var returnOrderCmd = new SqlCommand(returnOrderQuery, connection, transaction);
                returnOrderCmd.Parameters.AddWithValue("@OrderId", orderId);
                returnOrderCmd.ExecuteNonQuery();

                // Update the copies available
                string returnCopiesQuery = @"
                UPDATE Copies
                SET NumberOfCopies = NumberOfCopies + o.RequestedCopiesQTY
                FROM Copies c
                INNER JOIN Orders o ON c.BookId = o.BookId AND c.LibraryId = o.LibraryId
                WHERE o.OrderId = @OrderId";
                using var returnCopiesCmd = new SqlCommand(returnCopiesQuery, connection, transaction);
                returnCopiesCmd.Parameters.AddWithValue("@OrderId", orderId);
                returnCopiesCmd.ExecuteNonQuery();

                // Commit transaction
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                // Rollback in case of error
                await transaction.RollbackAsync();
                throw new Exception($"Error returning book: {e.Message}", e);
            }
        }

    }
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
