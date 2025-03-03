using Microsoft.Data.SqlClient;
using LibDB;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class RequestBookService
    {
        private readonly string CnString;

        public RequestBookService()
        {
            CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
            //CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        private bool CanRequest(int userId, int numberOfCopies)
        {
            bool canRequest = true;
            int numberOfCopiesOrdered = 0;

            try
            {
                List<Order> userOders = new Orders().GetOrdersByUserId(userId);

                foreach (Order order in userOders)
                {
                    if (!order.ReturnDate.HasValue)
                    {
                        numberOfCopiesOrdered += order.RequestedCopiesQTY;
                    }
                }

                // if the total number of copies that the user has already ordered is superior or equal to 4
                // OR 
                // if the numberOfCopiesOrdered plus the numberOfCopies that he wants to order is superior than 4 (if it's equal he can still order)
                if (numberOfCopiesOrdered >= 4 || numberOfCopiesOrdered + numberOfCopies > 4)
                {
                    canRequest = false;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return canRequest;
        }

        public async Task<bool> RequestBook(int userId, int bookId, int libraryId, int numberOfCopies)
        {

            if (!CanRequest(userId, numberOfCopies))
                throw new Exception("Can't request more than 4 copies.");

            using SqlConnection connection = DB.Open(CnString);
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

                // Fetch additional details
                var userDetails = await GetUserDetails(connection, transaction, userId);
                var bookDetails = await GetBookDetails(connection, transaction, bookId);
                var libraryDetails = await GetLibraryDetails(connection, transaction, libraryId);

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

                await transaction.CommitAsync();
                return true;

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception(e.Message, e.InnerException);
            }
        }

        private async Task<UserDetails> GetUserDetails(SqlConnection connection, SqlTransaction transaction, int userId)
        {
            string query = "SELECT FirstName, LastName FROM Users WHERE UserId = @userId";
            using SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@userId", userId);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string firstName = reader["FirstName"].ToString();
                string lastName = reader["LastName"].ToString();
                string userName = $"{firstName} {lastName}";

                return new UserDetails
                {
                    UserName = userName
                };
            }
            throw new Exception("User not found.");
        }

        private async Task<BookDetails> GetBookDetails(SqlConnection connection, SqlTransaction transaction, int bookId)
        {
            // Fetch book details from the Books table
            string bookQuery = @"
        SELECT Title, Edition, Year, AuthorId 
        FROM Books 
        WHERE BookId = @bookId";
            using SqlCommand bookCmd = new SqlCommand(bookQuery, connection, transaction);
            bookCmd.Parameters.AddWithValue("@bookId", bookId);
            using SqlDataReader bookReader = await bookCmd.ExecuteReaderAsync();

            if (await bookReader.ReadAsync())
            {
                // Fetch book details
                string title = bookReader["Title"].ToString();
                string edition = bookReader["Edition"].ToString();
                int year = Convert.ToInt32(bookReader["Year"]);
                int authorId = Convert.ToInt32(bookReader["AuthorId"]);

                await bookReader.CloseAsync();

                // Fetch author details from the Authors table
                string authorQuery = "SELECT AuthorName FROM Authors WHERE AuthorId = @authorId";
                using SqlCommand authorCmd = new SqlCommand(authorQuery, connection, transaction);
                authorCmd.Parameters.AddWithValue("@authorId", authorId);
                using SqlDataReader authorReader = await authorCmd.ExecuteReaderAsync();

                if (await authorReader.ReadAsync())
                {
                    string authorName = authorReader["AuthorName"].ToString();

                    // Return the combined book and author details
                    return new BookDetails
                    {
                        BookName = title, 
                        BookYear = year,        
                        BookEdition = edition,  
                        BookAuthor = authorName 
                    };
                }
                else
                {
                    throw new Exception("Author not found.");
                }
            }
            else
            {
                throw new Exception("Book not found.");
            }
        }
        private async Task<LibraryDetails> GetLibraryDetails(SqlConnection connection, SqlTransaction transaction, int libraryId)
        {
            string query = "SELECT LibraryName FROM Libraries WHERE LibraryId = @libraryId";
            using SqlCommand cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@libraryId", libraryId);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LibraryDetails
                {
                    LibraryName = reader["LibraryName"].ToString()
                };
            }
            throw new Exception("Library not found.");
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

        public class UserDetails
        {
            public string UserName { get; set; }
        }

        public class BookDetails
        {
            public string BookName { get; set; }
            public int BookYear { get; set; }
            public string BookEdition { get; set; }
            public string BookAuthor { get; set; }
        }

        public class LibraryDetails
        {
            public string LibraryName { get; set; }
        }
    }
}
