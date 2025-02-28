using System.Data;
using System.Text.RegularExpressions;
using ADOLib.ModelView;
using EFLibrary.Models;
using LibDB;
using Microsoft.Data.SqlClient;
using static ADOLib.Model.Model;
using Book = ADOLib.Model.Model.Book;
using Library = ADOLib.Model.Model.Library;

namespace ADOLib
{
    public class Statistics
    {
        private string CnString { get; set; }

        public Statistics()
        {
            //CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
        CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }
        // Get most requested books
        public List<MostRequestedBooks> GetMostRequestedBooks()
        {
            List<MostRequestedBooks> books = new List<MostRequestedBooks>();

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = "SELECT Books.BookId, Books.Title, Orders.OrderId, " +
                        "Orders.RequestedCopiesQTY, SUM(Orders.RequestedCopiesQTY) AS TotalRequests " +
                        "FROM Books " +
                        "INNER JOIN Orders ON Books.BookId = Orders.BookId " +
                        "GROUP BY Books.BookId, Books.Title, Orders.OrderId, Orders.RequestedCopiesQTY " +
                        "ORDER BY TotalRequests DESC";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    foreach(DataRow row in dataTable.Rows)
                    {
                        MostRequestedBooks mostRequested = new MostRequestedBooks()
                        {
                            BookId = Convert.ToInt32(row["BookId"]),
                            Title = row["Title"].ToString(),
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            RequestedCopiesQTY = Convert.ToInt32(row["RequestedCopiesQTY"]),
                            TotalRequests = Convert.ToInt32(row["TotalRequests"])
                        };
                        books.Add(mostRequested);
                    }
                }

                return books;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        // Get books by subjects
        public List<Book> GetBooksBySubject(string subject)
        {
            List<Book> books = new List<Book>();
            Books bk = new Books();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = "SELECT BookId " +
                        "FROM Books " +
                        "INNER JOIN BookSubject ON Books.BookId = BookSubject.BooksBookId " +
                        "INNER JOIN Subjects ON BookSubject.SubjectsSubjectId = Subjects.SubjectId" +
                        $"WHERE Subject.SubjectName = '{subject}' ";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(row["BookId"]);
                        books.Add(bk.GetBookById(id));
                    }
                }

                return books;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        // Get libraries by book (libraries that contain a specific book)
        public List<Library> GetLibrariesByBook(Book book)
        {
            List<Library> libraries = new List<Library>();
            Libraries lib = new Libraries();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = "SELECT * " +
                        "FROM Libraries " +
                        "INNER JOIN Copies ON Libraries.LibraryId = Copies.LibraryId " +
                        "INNER JOIN Books ON Books.BookId = Copies.BookId" +
                        $"WHERE Books.BookId = '{book.BookId}' ";

                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(row["LibraryId"]);
                        libraries.Add(lib.GetLibraryById(id));
                    }
                }

                return libraries;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
