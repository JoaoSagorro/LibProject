using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOLib.Enums;
using ADOLib.ModelView;
using EFLibrary;
using LibDB;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class Orders
    {
        private string CnString { get; set; }

        public Orders()
        {
            //CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
            CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        public List<UserOrder> CheckOrderState(int userId)
        {
            List<UserOrder> orders = new List<UserOrder>();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = @"
                        SELECT 
                            Orders.OrderId,
                            Books.Title,
                            Authors.AuthorName,
                            Libraries.LibraryName,
                            Orders.OrderDate,
                            Orders.ReturnDate,
                            States.StateName
                        FROM Orders
                        INNER JOIN Books ON Orders.BookId = Books.BookId
                        INNER JOIN Authors ON Books.AuthorId = Authors.AuthorId
                        INNER JOIN Libraries ON Orders.LibraryId = Libraries.LibraryId
                        INNER JOIN States ON Orders.StateId = States.StateId
                        WHERE Orders.UserId = " + userId.ToString() + @"
                        GROUP BY 
                            Orders.OrderId,
                            Books.Title,
                            Authors.AuthorName,
                            Libraries.LibraryName,
                            Orders.OrderDate,
                            Orders.ReturnDate,
                            States.StateName
                        Order BY
                            Orders.OrderDate";

                    string updateQuery = $"UPDATE Orders SET StateId = @stateId WHERE OrderId = @orderId";
                    DataTable dataTable = DB.GetSQLRead(connection, query);
                    States states = new States();

                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        foreach (DataRow order in dataTable.Rows)
                        {
                            int dayDiff = (DateTime.Now - Convert.ToDateTime(order["OrderDate"])).Days;

                            int id = Convert.ToInt32(order["OrderId"]);
                            cmd.Parameters.AddWithValue("@orderId", id);
                            int stateId = (int)StatesEnum.Solicitado;

                            if (order["ReturnDate"] != DBNull.Value)
                            {
                                stateId = (int)StatesEnum.Devolvido;
                            }
                            else if (dayDiff > 15)
                            {
                                stateId = (int)StatesEnum.EmAtraso;
                            }
                            else if (dayDiff > 12)
                            {
                                stateId = (int)StatesEnum.DevolucaoUrgente;
                            }
                            else if (dayDiff > 10)
                            {
                                stateId = (int)StatesEnum.DevolucaoEmBreve;
                            }

                            UserOrder newOrder = new UserOrder()
                            {
                                OrderId = Convert.ToInt32(order["OrderId"]),
                                Title = order["Title"].ToString(),
                                AuthorName = order["AuthorName"].ToString(),
                                LibraryName = order["LibraryName"].ToString(),
                                OrderDate = Convert.ToDateTime(order["OrderDate"]),
                                ReturnDate = order["ReturnDate"] != DBNull.Value ? Convert.ToDateTime(order["ReturnDate"]) : DateTime.MinValue,
                                StateName = states.GetStateById(stateId).StateName
                            };

                            orders.Add(newOrder);

                            cmd.Parameters.AddWithValue("@stateId", stateId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return orders;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public Order GetOrderById(int orderId)
        {
            Order order = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Orders WHERE OrderId = {orderId}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) return order;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        order = new Order()
                        {
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            BookId = Convert.ToInt32(row["BookId"]),
                            UserId = Convert.ToInt32(row["UserId"]),
                            LibraryId = Convert.ToInt32(row["LibraryId"]),
                            StateId = Convert.ToInt32(row["StateId"]),
                            OrderDate = Convert.ToDateTime(row["OrderDate"]),
                            ReturnDate = Convert.ToDateTime(row["ReturnDate"]),
                        };
                    }
                }
            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return order;
        } 

        public List<Order> GetOrdersByUserId(int userId)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Orders WHERE UserId = {userId}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count == 0) return orders;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Order order = new Order()
                        {
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            BookId = Convert.ToInt32(row["BookId"]),
                            UserId = Convert.ToInt32(row["UserId"]),
                            LibraryId = Convert.ToInt32(row["LibraryId"]),
                            StateId = Convert.ToInt32(row["StateId"]),
                            OrderDate = Convert.ToDateTime(row["OrderDate"]),
                            ReturnDate = Convert.ToDateTime(row["ReturnDate"]),
                        };

                        orders.Add(order);
                    }
                }
            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return orders;
        }


        public List<Order> DeleteUserOrders(int userId)
        {
            List<Order> orders = null;

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    orders = GetOrdersByUserId(userId);
                    string deleteOrders = "DELETE FROM Orders WHERE Orders.UserId = @userId";
                    SqlTransaction transaction = connection.BeginTransaction();
                    using(SqlCommand cmd = new SqlCommand(deleteOrders, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        int affectedRows = cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return orders;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        private bool CanRequest(int numberOfCopies) { return numberOfCopies <= 4; }

        public async Task<bool> RequestBook(int userId, int bookId, int libraryId, int numberOfCopies)
        {
            using var connection = new SqlConnection(CnString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                if (!CanRequest(numberOfCopies)) throw new Exception("Can't request the specified amount of copies.");

                if (!await ExistsAsync(connection, transaction, "Users", "UserId", userId) ||
                    !await ExistsAsync(connection, transaction, "Libraries", "LibraryId", libraryId) ||
                    !await ExistsAsync(connection, transaction, "Books", "BookId", bookId))
                {
                    throw new InvalidOperationException("Invalid request. User, Library, or Book not found.");
                }

                var copyCount = await GetCopyCountAsync(connection, transaction, bookId, libraryId);

                // check if copies remaining are >= 1
                int remainingCopies = copyCount - numberOfCopies;
                if (remainingCopies < 1) throw new Exception("Can't request the specified amount of copies.");

                if (copyCount <=  1)
                {
                    throw new InvalidOperationException("Book not available.");
                }

                DateTime orderDate = DateTime.UtcNow;
                int orderId = await InsertOrderAsync(connection, transaction, userId, bookId, libraryId, orderDate);

                await UpdateCopyCountAsync(connection, transaction, bookId, libraryId, remainingCopies);

                await InsertOrderHistoryAsync(connection, transaction, userId, bookId, libraryId, orderDate);

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

        private async Task<int> InsertOrderAsync(SqlConnection conn, SqlTransaction transaction, int userId, int bookId, int libraryId, DateTime orderDate, DateTime? returnDate = null)
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

        private async Task InsertOrderHistoryAsync(SqlConnection conn, SqlTransaction transaction, int userId, int bookId, int libraryId, DateTime orderDate, DateTime? returnDate = null)
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
