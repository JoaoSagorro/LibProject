using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            CnString = "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";
        }

        

        public bool OrderBook(int userId, int bookId, int libraryId)
        {
            try
            {
                // Load all data needed for the transaction
                using (SqlConnection connection = DB.Open(CnString))
                {
                    User user = new Users().GetUserInfo(userId);
                    Library library = new Libraries().GetLibraryById(libraryId);
                    Book book = new Books().GetBookById(bookId);
                    Author author = new Authors().GetAuthorById(book.AuthorId);

                    if (user == null || library == null || book == null)
                    {
                        throw new InvalidOperationException("Invalid request. User, Library, or Book not found.");
                    }

                    Copie copie = new Copies().GetCopies(bookId, libraryId);

                    if (copie == null || copie.NumberOfCopies < 1)
                    {
                        throw new InvalidOperationException("Book not available.");
                    }

                    Order order = new Order()
                    {
                        UserId = user.UserId,
                        LibraryId = library.LibraryId,
                        BookId = book.BookId,
                        StateId = 1,
                        OrderDate = DateTime.UtcNow,
                        ReturnDate = DateTime.UtcNow.AddDays(15) // shouldn't it be null?????
                    };

                    SqlTransaction transaction = connection.BeginTransaction();

                    string orderQuery = $"INSERT INTO Orders (UserId, LibraryId, BookId, StateId, OrderDate, ReturnDate) " +
                        $"VALUES ({order.UserId}, {order.LibraryId}, {order.BookId}, {order.StateId}, {order.OrderDate}, {order.ReturnDate})";

                    transaction.Commit();

                    // don't forget to change the number of copies requested
                    Copies.ChangeNumberOfCopies(copie, 1);

                    var orderHistory = new OrderHistory
                    {
                        UserName = user.FirstName,
                        BookName = book.Title,
                        BookYear = book.Year,
                        BookAuthor = author.AuthorName,
                        BookEdition = book.Edition,
                        LibraryName = library.LibraryName,
                        OrderedCopies = 1,
                        OrderDate = order.OrderDate,
                        ReturnDate = order.ReturnDate
                    };

                    OrdersHistory.AddHistory(CnString, orderHistory);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred.", ex);
            }
        }

    }
}
