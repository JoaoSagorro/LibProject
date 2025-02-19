using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ADOLib
{
    public class RequestBookService
    {
        private string CnString { get; set; }

        public RequestBookService(string connectionString)
        {
            CnString = "";
        }

        public async Task<bool> RequestBook(int userId, int bookId, int libraryId)
        {
            using var connection = new SqlConnection(CnString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {

                if (!await ExistsAsync(connection, transaction, "Users", "UserId", userId) ||
                    !await ExistsAsync(connection, transaction, "Libraries", "LibraryId", libraryId) ||
                    !await ExistsAsync(connection, transaction, "Books", "BookId", bookId))
                {
                    throw new InvalidOperationException("Invalid request. User, Library, or Book not found.");
                }

                var copyCount = await GetCopyCountAsync(connection, transaction, bookId, libraryId);
                if (copyCount < 1)
                {
                    throw new InvalidOperationException("Book not available.");
                }

                DateTime orderDate = DateTime.UtcNow;
                DateTime returnDate = orderDate.AddDays(15);
                int orderId = await InsertOrderAsync(connection, transaction, userId, bookId, libraryId, orderDate, returnDate);

                await UpdateCopyCountAsync(connection, transaction, bookId, libraryId, copyCount - 1);

                await InsertOrderHistoryAsync(connection, transaction, userId, bookId, libraryId, orderDate, returnDate);

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

        private async Task<bool> ExistsAsync(SqlConnection conn, SqlTransaction transaction, string table, string column, int id)
        {
            string query = $"SELECT COUNT(1) FROM {table} WHERE {column} = @Id";
            using var cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@Id", id);
            return (int)await cmd.ExecuteScalarAsync() > 0;
        }

        private async Task<int> GetCopyCountAsync(SqlConnection conn, SqlTransaction transaction, int bookId, int libraryId)
        {
            string query = "SELECT NumberOfCopies FROM Copies WHERE BookId = @BookId AND LibraryId = @LibraryId";
            using var cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@LibraryId", libraryId);
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        private async Task<int> InsertOrderAsync(SqlConnection conn, SqlTransaction transaction, int userId, int bookId, int libraryId, DateTime orderDate, DateTime returnDate)
        {
            string query = @"INSERT INTO Orders (UserId, BookId, LibraryId, OrderDate, ReturnDate) 
                             OUTPUT INSERTED.OrderId VALUES (@UserId, @BookId, @LibraryId, @OrderDate, @ReturnDate)";
            using var cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@LibraryId", libraryId);
            cmd.Parameters.AddWithValue("@OrderDate", orderDate);
            cmd.Parameters.AddWithValue("@ReturnDate", returnDate);
            return (int)await cmd.ExecuteScalarAsync();
        }

        private async Task UpdateCopyCountAsync(SqlConnection conn, SqlTransaction transaction, int bookId, int libraryId, int newCount)
        {
            string query = "UPDATE Copies SET NumberOfCopies = @NewCount WHERE BookId = @BookId AND LibraryId = @LibraryId";
            using var cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@NewCount", newCount);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@LibraryId", libraryId);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task InsertOrderHistoryAsync(SqlConnection conn, SqlTransaction transaction, int userId, int bookId, int libraryId, DateTime orderDate, DateTime returnDate)
        {
            string query = @"INSERT INTO OrderHistories (UserName, BookName, BookYear, BookAuthor, BookEdition, LibraryName, OrderedCopies, OrderDate, ReturnDate)
                             SELECT 
                                 u.FirstName, 
                                 b.Title, 
                                 b.Year, 
                                 a.AuthorName, 
                                 b.Edition, 
                                 l.LibraryName, 
                                 1, 
                                 @OrderDate, 
                                 @ReturnDate
                             FROM Users u 
                             JOIN Books b ON b.BookId = @BookId
                             JOIN Authors a ON a.AuthorId = b.AuthorId
                             JOIN Libraries l ON l.LibraryId = @LibraryId
                             WHERE u.UserId = @UserId";
            using var cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@LibraryId", libraryId);
            cmd.Parameters.AddWithValue("@OrderDate", orderDate);
            cmd.Parameters.AddWithValue("@ReturnDate", returnDate);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}

