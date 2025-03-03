using ADOLib.Enums;
using LibDB;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ADOLib
{
    public class ReturnBookService
    {
        private readonly string CnString;

        public ReturnBookService()
        {
            CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        //public async void ReturnBookByOrderId(int orderId)
        //{
        //    using var connection = DB.Open(CnString);
        //    using var transaction = connection.BeginTransaction();

        //    try
        //    {
        //        // Check if the order exists and if it's overdue
        //        string checkOrderQuery = "SELECT OrderDate FROM Orders WHERE OrderId = @OrderId";
        //        using var checkOrderCmd = new SqlCommand(checkOrderQuery, connection, transaction);
        //        checkOrderCmd.Parameters.AddWithValue("@OrderId", orderId);
        //        var orderDate = (DateTime)checkOrderCmd.ExecuteScalar();

        //        if ((DateTime.UtcNow - orderDate).Days > 15)
        //        {
        //            var user = new Users();
        //            int strikes = user.StrikeUser(orderId);

        //            if (strikes > 3)
        //            {
        //                string suspendQuery = @"
        //            UPDATE Users SET Suspended = 1, Active = 0
        //            WHERE UserId IN (SELECT UserId FROM Orders WHERE OrderId = @OrderId)";
        //                using var suspendCmd = new SqlCommand(suspendQuery, connection, transaction);
        //                suspendCmd.Parameters.AddWithValue("@OrderId", orderId);
        //                suspendCmd.ExecuteNonQuery();
        //            }
        //        }

        //        // Update the order return date
        //        string returnOrderQuery = $"UPDATE Orders SET ReturnDate = GETDATE(), StateId = {(int)StatesEnum.Devolvido} WHERE OrderId = @OrderId";
        //        using var returnOrderCmd = new SqlCommand(returnOrderQuery, connection, transaction);
        //        returnOrderCmd.Parameters.AddWithValue("@OrderId", orderId);
        //        returnOrderCmd.ExecuteNonQuery();

        //        // Update the copies available
        //        string returnCopiesQuery = @"
        //        UPDATE Copies
        //        SET NumberOfCopies = NumberOfCopies + o.RequestedCopiesQTY
        //        FROM Copies c
        //        INNER JOIN Orders o ON c.BookId = o.BookId AND c.LibraryId = o.LibraryId
        //        WHERE o.OrderId = @OrderId";
        //        using var returnCopiesCmd = new SqlCommand(returnCopiesQuery, connection, transaction);
        //        returnCopiesCmd.Parameters.AddWithValue("@OrderId", orderId);
        //        returnCopiesCmd.ExecuteNonQuery();

        //        // Insert into order history
        //        await InsertOrderHistory(connection, transaction, orderId);

        //        // Commit transaction
        //        await transaction.CommitAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        // Rollback in case of error
        //        await transaction.RollbackAsync();
        //        throw new Exception($"Error returning book: {e.Message}", e);
        //    }
        //}

        public void ReturnBookByOrderId(int orderId)
        {
            using var connection = DB.Open(CnString);
            var transaction = connection.BeginTransaction();

            try
            {
                string returnOrder = $"Update Orders Set ReturnDate = GETDATE() , StateId = {(int)StatesEnum.Devolvido} WHERE OrderId = {orderId}";
                string returnCopies = $@"UPDATE c
                                        SET c.NumberOfCopies = c.NumberOfCopies + o.RequestedCopiesQTY
                                        FROM Copies c
                                        INNER JOIN Orders o ON c.BookId = o.BookId AND c.LibraryId = o.LibraryId
                                        WHERE o.OrderId = {orderId};";
                string isOverdue = $"SELECT OrderDate FROM Orders o WHERE o.OrderId = {orderId};";
                DB.CmdExecute(connection, returnCopies, transaction);
                DB.CmdExecute(connection, returnOrder, transaction);
                DataTable overdue = DB.GetSQLRead(connection, isOverdue, transaction);
                DateTime date = Convert.ToDateTime(overdue.Rows[0]["OrderDate"]);
                if ((DateTime.UtcNow - date).Days > 15)
                {
                    var user = new Users();
                    int strikes = user.StrikeUser(orderId, transaction);
                    if (strikes > 3)
                    {
                        string suspendQuery = $@"UPDATE u
                                                SET u.Suspended=1, u.Active=0
                                                FROM Users u
                                                INNER JOIN Orders o ON o.UserId = u.UserId
                                                WHERE o.OrderId = {orderId};";
                        var userId = int.Parse(DB.GetSQLRead(connection, suspendQuery,transaction).Rows[0]["userId"].ToString());

                    }
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception($"Error returning book: {e}");
            }
        }

        public void ReturnBookByOrderId(int orderId, SqlTransaction transaction)
        {
            using var connection = DB.Open(CnString);

            try
            {
                string returnOrder = $"Update Orders Set ReturnDate = GETDATE() , StateId = {(int)StatesEnum.Devolvido} WHERE OrderId = {orderId}";
                string returnCopies = $@"UPDATE c
                                        SET c.NumberOfCopies = c.NumberOfCopies + o.RequestedCopiesQTY
                                        FROM Copies c
                                        INNER JOIN Orders o ON c.BookId = o.BookId AND c.LibraryId = o.LibraryId
                                        WHERE o.OrderId = {orderId};";
                string isOverdue = $"SELECT OrderDate FROM Orders o WHERE o.OrderId = {orderId};";
                DB.CmdExecute(connection, returnCopies, transaction);
                DB.CmdExecute(connection, returnOrder, transaction);
                DataTable overdue = DB.GetSQLRead(connection, isOverdue, transaction);
                DateTime date = Convert.ToDateTime(overdue.Rows[0]["OrderDate"]);
                if ((DateTime.UtcNow - date).Days > 15)
                {
                    var user = new Users();
                    int strikes = user.StrikeUser(orderId, transaction);
                    if (strikes > 3)
                    {
                        string suspendQuery = $@"UPDATE u
                                                SET u.Suspended=1, u.Active=0
                                                FROM Users u
                                                INNER JOIN Orders o ON o.UserId = u.UserId
                                                WHERE o.OrderId = {orderId};";
                        var userId = int.Parse(DB.GetSQLRead(connection, suspendQuery, transaction).Rows[0]["userId"].ToString());

                    }
                }
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception($"Error returning book: {e}");
            }
        }

        private async Task InsertOrderHistory(SqlConnection connection, SqlTransaction transaction, int orderId)
        {
            // Fetch order details
            string fetchOrderQuery = @"
        SELECT u.FirstName, u.LastName, b.Title, b.Edition, b.Year, a.AuthorName, l.LibraryName, o.OrderDate, o.RequestedCopiesQTY
        FROM Orders o
        JOIN Users u ON o.UserId = u.UserId
        JOIN Books b ON o.BookId = b.BookId
        JOIN Authors a ON b.AuthorId = a.AuthorId
        JOIN Libraries l ON o.LibraryId = l.LibraryId
        WHERE o.OrderId = @orderId";

            using SqlCommand fetchCmd = new SqlCommand(fetchOrderQuery, connection, transaction);
            fetchCmd.Parameters.AddWithValue("@orderId", orderId);

            using SqlDataReader reader = await fetchCmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                throw new Exception("Order not found.");

            string userName = $"{reader["FirstName"]} {reader["LastName"]}";
            string bookName = reader["Title"].ToString();
            string bookEdition = reader["Edition"].ToString();
            int bookYear = Convert.ToInt32(reader["Year"]);
            string bookAuthor = reader["AuthorName"].ToString();
            string libraryName = reader["LibraryName"].ToString();
            DateTime orderDate = Convert.ToDateTime(reader["OrderDate"]);
            int orderedCopies = Convert.ToInt32(reader["RequestedCopiesQTY"]);

            await reader.CloseAsync();

            // Insert into OrderHistories
            string insertHistoryQuery = @"
        INSERT INTO OrderHistories (UserName, BookName, BookYear, BookEdition, BookAuthor, LibraryName, OrderedCopies, OrderDate, ReturnDate)
        VALUES (@userName, @bookName, @bookYear, @bookEdition, @bookAuthor, @libraryName, @orderedCopies, @orderDate, @returnDate)";

            using SqlCommand insertCmd = new SqlCommand(insertHistoryQuery, connection, transaction);
            insertCmd.Parameters.AddWithValue("@userName", userName);
            insertCmd.Parameters.AddWithValue("@bookName", bookName);
            insertCmd.Parameters.AddWithValue("@bookYear", bookYear);
            insertCmd.Parameters.AddWithValue("@bookEdition", bookEdition);
            insertCmd.Parameters.AddWithValue("@bookAuthor", bookAuthor);
            insertCmd.Parameters.AddWithValue("@libraryName", libraryName);
            insertCmd.Parameters.AddWithValue("@orderedCopies", orderedCopies);
            insertCmd.Parameters.AddWithValue("@orderDate", orderDate);
            insertCmd.Parameters.AddWithValue("@returnDate", DateTime.UtcNow);

            await insertCmd.ExecuteNonQueryAsync();
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
