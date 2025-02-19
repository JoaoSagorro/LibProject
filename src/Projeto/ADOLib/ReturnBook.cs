using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LibLibrary.Services
{
    public class ReturnBookService
    {
        private string CnString { get; set; }

        public ReturnBookService(string connectionString)
        {
            CnString = connectionString;
        }

        public async Task<bool> ReturnBook(int userId, int orderId)
        {
            using SqlConnection connection = new SqlConnection(CnString);
            await connection.OpenAsync();
            SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                // Check if the order exists for the given user
                string checkOrderQuery = @"
                            SELECT o.OrderId, o.BookId, o.LibraryId 
                            FROM Orders o
                            JOIN Users u ON o.UserId = u.UserId
                            WHERE o.OrderId = @OrderId AND u.UserId = @UserId";

                int bookId, libraryId;
                using (SqlCommand cmd = new SqlCommand(checkOrderQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException("Order not found or user does not have this order.");
                    }

                    bookId = reader.GetInt32(1);
                    libraryId = reader.GetInt32(2);
                }

                // Check if the book copy exists in the library
                string checkCopyQuery = @"
                            SELECT NumberOfCopies FROM Copies 
                            WHERE BookId = @BookId AND LibraryId = @LibraryId";

                int numberOfCopies;
                using (SqlCommand cmd = new SqlCommand(checkCopyQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@BookId", bookId);
                    cmd.Parameters.AddWithValue("@LibraryId", libraryId);

                    object result = await cmd.ExecuteScalarAsync();
                    if (result == null)
                    {
                        throw new InvalidOperationException("Book copy not found in library.");
                    }
                    numberOfCopies = Convert.ToInt32(result);
                }

                // Update order return date
                string updateOrderQuery = @"
                            UPDATE Orders SET ReturnDate = @ReturnDate WHERE OrderId = @OrderId";

                using (SqlCommand cmd = new SqlCommand(updateOrderQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@ReturnDate", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    await cmd.ExecuteNonQueryAsync();
                }

                // Increase book copies in library
                string updateCopiesQuery = @"
                            UPDATE Copies SET NumberOfCopies = @NewCount WHERE BookId = @BookId AND LibraryId = @LibraryId";

                using (SqlCommand cmd = new SqlCommand(updateCopiesQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@NewCount", numberOfCopies + 1);
                    cmd.Parameters.AddWithValue("@BookId", bookId);
                    cmd.Parameters.AddWithValue("@LibraryId", libraryId);
                    await cmd.ExecuteNonQueryAsync();
                }

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
