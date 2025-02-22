using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.Data.SqlClient;
using LibDB;

namespace ADOLib
{
    internal class RequestBookService
    {
        private readonly string _cnString;

        public RequestBookService()
        {
            _cnString = "Server=;Database=;Trusted_Connection=True;TrustServerCertificate=True";
        }

        private bool CanRequest(int numberOfCopies) => numberOfCopies <= 4;

        public async Task<bool> RequestBook(int userId, int bookId, int libraryId, int numberOfCopies)
        {
            if (!CanRequest(numberOfCopies))
                throw new Exception("Can't request more than 4 copies.");

            using SqlConnection connection = DB.Open(_cnString);
            await connection.OpenAsync();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                // Validate entities
                var userExists = await CheckExists(connection, transaction, "Users", "UserId", userId);
                var libraryExists = await CheckExists(connection, transaction, "Libraries", "LibraryId", libraryId);
                var bookExists = await CheckExists(connection, transaction, "Books", "BookId", bookId);
                var stateId = 1;
                if (!userExists || !libraryExists || !bookExists)
                    throw new InvalidOperationException("Invalid request. User, Library, or Book not found.");

                // Check if book is available
                int availableCopies = await GetAvailableCopies(connection, transaction, bookId, libraryId);
                if (availableCopies < numberOfCopies || availableCopies <= 1)
                    throw new InvalidOperationException("Book not available.");

                // Insert order
                string orderQuery = "INSERT INTO Orders (UserId, BookId, LibraryId, StateId, OrderDate, RequestedCopiesQTY) OUTPUT INSERTED.OrderId " +
                               "VALUES (@userId, @bookId, @libraryId, @stateId, @orderDate, @requestedCopiesQTY)";
                using SqlCommand orderCmd = new SqlCommand(orderQuery, connection, transaction);
                orderCmd.Parameters.AddWithValue("@userId", userId);
                orderCmd.Parameters.AddWithValue("@bookId", bookId);
                orderCmd.Parameters.AddWithValue("@libraryId", libraryId);
                orderCmd.Parameters.AddWithValue("@stateId", stateId);
                orderCmd.Parameters.AddWithValue("@orderDate", DateTime.UtcNow);
                orderCmd.Parameters.AddWithValue("@requestedCopiesQTY", numberOfCopies);
                int orderId = (int)await orderCmd.ExecuteScalarAsync();

                // Update copies
                string updateCopiesQuery = "UPDATE Copies SET NumberOfCopies = NumberOfCopies - @RequestedCopiesQTY WHERE BookId = @bookId AND LibraryId = @libraryId";
                using SqlCommand updateCopiesCmd = new SqlCommand(updateCopiesQuery, connection, transaction);
                updateCopiesCmd.Parameters.AddWithValue("@RequestedCopiesQTY", numberOfCopies);
                updateCopiesCmd.Parameters.AddWithValue("@bookId", bookId);
                updateCopiesCmd.Parameters.AddWithValue("@libraryId", libraryId);
                await updateCopiesCmd.ExecuteNonQueryAsync();

                // Insert into order history
                string orderHistoryQuery = "INSERT INTO OrderHistory (UserId , BookId, LibraryId, OrderedCopies, OrderDate) VALUES (@userId, @bookId, @libraryId, @orderedCopies, @orderDate)";
                using SqlCommand orderHistoryCmd = new SqlCommand(orderHistoryQuery, connection, transaction);
                orderHistoryCmd.Parameters.AddWithValue("@userId", userId);
                orderHistoryCmd.Parameters.AddWithValue("@bookId", bookId);
                orderHistoryCmd.Parameters.AddWithValue("@libraryId", libraryId);
                orderHistoryCmd.Parameters.AddWithValue("@orderedCopies", numberOfCopies);
                orderHistoryCmd.Parameters.AddWithValue("@orderDate", DateTime.UtcNow);
                await orderHistoryCmd.ExecuteNonQueryAsync();

                // Commit transaction
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception(e.Message, e.InnerException);
            }
        }

        private async Task<bool> CheckExists(SqlConnection connection, SqlTransaction transaction, string tableName, string columnName, int id)
        {
            string query = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @id";
            using SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@id", id);
            object result = await cmd.ExecuteScalarAsync();
            int count = result != null ? Convert.ToInt32(result) : 0;
            return count > 0;
        }

        private async Task<int> GetAvailableCopies(SqlConnection connection, SqlTransaction transaction, int bookId, int libraryId)
        {
            string query = "SELECT NumberOfCopies FROM Copies WHERE BookId = @bookId AND LibraryId = @libraryId";
            using SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@bookId", bookId);
            cmd.Parameters.AddWithValue("@libraryId", libraryId);
            object result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
