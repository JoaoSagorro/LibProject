using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ADOLib.ModelView;
using LibDB;
using Microsoft.Data.SqlClient;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class Books
    {
        private string CnString { get; set; }

        public Books()
        {
            //CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
        CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        // Make a method called CreateBooksInfo where it gathers all the info from all the books and returns a list of it.

        //private string QueryCreator(int? id = null)
        //{
        //    string query = "SELECT Books.BookId, " +
        //                "Books.title, Books.Edition, Books.Year, Books.Quantity, " +
        //                "Authors.AuthorId, Authors.AuthorName, " +
        //                "Libraries.LibraryId, Libraries.LibraryName, Libraries.LibraryAddress, Libraries.Email, Libraries.Contact, " +
        //                "Copies.NumberOfCopies, Covers.CoverImage, " +
        //                "STRING_AGG(Subjects.SubjectName, ', ') AS SubjectNames " +
        //                "FROM Books " +
        //                "INNER JOIN Copies ON Books.BookId = Copies.BookId " +
        //                "INNER JOIN Libraries ON Copies.LibraryId = Libraries.LibraryId " +
        //                "INNER JOIN Authors ON Books.AuthorId = Authors.AuthorId " +
        //                "INNER JOIN BookSubject ON Books.BookId = BookSubject.BooksBookId " +
        //                "INNER JOIN Covers ON Books.BookId = Covers.BookId " +
        //                "INNER JOIN Subjects ON BookSubject.SubjectsSubjectId = Subjects.SubjectId ";

        //    if(id is not null)
        //    {
        //        query += $"WHERE Books.BookId = {id} ";
        //    }

        //    query += "GROUP BY Books.BookId, Books.Title, Books.Edition, Books.Year, Books.Quantity, " +
        //        "Authors.AuthorId, Authors.AuthorName, " +
        //        "Libraries.LibraryId, Libraries.LibraryName, Libraries.LibraryAddress, Libraries.Email, Libraries.Contact, " +
        //        "Copies.NumberOfCopies, Covers.CoverImage";

        //    return query;
        //}

        public Book GetBookById(int id)
        {
            Book book = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Books WHERE BookId = {id}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) throw new Exception("An error has occurred when trying to find the book.");

                    foreach (DataRow row in dataTable.Rows)
                    {
                        book = new Book()
                        {
                            BookId = Convert.ToInt32(row["BookId"]),
                            Title = row["Title"].ToString(),
                            Edition = row["Edition"].ToString(),
                            Year = Convert.ToInt32(row["Year"]),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                        };
                    }
                }

            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return book;
        }

        public List<BooksInfo> GetAllBooks(int? id = null)
        {
            List<BooksInfo> books = new List<BooksInfo>();

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = @"
                    SELECT 
                        Books.BookId, 
                        Books.Title, 
                        Books.Edition, 
                        Books.Year, 
                        Books.Quantity, 
                        Authors.AuthorId, 
                        Authors.AuthorName, 
                        Libraries.LibraryId, 
                        Libraries.LibraryName, 
                        Libraries.LibraryAddress, 
                        Libraries.Email, 
                        Libraries.Contact, 
                        Copies.NumberOfCopies, 
                        STRING_AGG(Subjects.SubjectName, ', ') AS SubjectNames 
                    FROM Books 
                    INNER JOIN Copies ON Books.BookId = Copies.BookId 
                    INNER JOIN Libraries ON Copies.LibraryId = Libraries.LibraryId 
                    INNER JOIN Authors ON Books.AuthorId = Authors.AuthorId 
                    INNER JOIN BookSubject ON Books.BookId = BookSubject.BooksBookId 
                    INNER JOIN Subjects ON BookSubject.SubjectsSubjectId = Subjects.SubjectId 
                    GROUP BY 
                        Books.BookId, 
                        Books.Title, 
                        Books.Edition, 
                        Books.Year, 
                        Books.Quantity, 
                        Authors.AuthorId, 
                        Authors.AuthorName, 
                        Libraries.LibraryId, 
                        Libraries.LibraryName, 
                        Libraries.LibraryAddress, 
                        Libraries.Email, 
                        Libraries.Contact, 
                        Copies.NumberOfCopies";

                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var book = new BooksInfo()
                        {
                            BookId = Convert.ToInt32(row["BookId"]),
                            Title = row["Title"].ToString(),
                            Edition = row["Edition"].ToString(),
                            Year = Convert.ToInt32(row["Year"]),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                            LibraryId = Convert.ToInt32(row["LibraryId"]),
                            LibraryName = row["LibraryName"].ToString(),
                            LibraryAddress = row["LibraryAddress"].ToString(),
                            Email = row["Email"].ToString(),
                            Contact = row["Contact"].ToString(),
                            NumberOfCopies = Convert.ToInt32(row["NumberOfCopies"]),
                            AuthorName = row["AuthorName"].ToString(),
                            SubjectNames = row["SubjectNames"].ToString().Split(',').Select(lst => lst.Trim()).ToList(),
                        };

                        books.Add(book);
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return books;
        }

        // Still not finished
        public void AddBook(BooksInfo book)
        {
            Authors authors = new Authors();
            Author author = null;

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    SqlTransaction transaction = connection.BeginTransaction();

                    if (BookFinder(book.Title, book.Edition) is not null) throw new Exception("The book already exists.");

                    if (authors.GetAuthorByName(book.AuthorName) != null)
                    {
                        author = authors.GetAuthorByName(book.AuthorName);
                    };

                    if (authors.GetAuthorByName(book.AuthorName) == null)
                    {
                        author = new Author()
                        {
                            AuthorName = book.AuthorName,
                        };

                        authors.AddAuthor(author);
                    }
                    
                    // First, add book to Books table;
                    string addBook = $"INSERT INTO Books (Title, Edition, Year, Quantity, AuthorId) " +
                        $"VALUES ({book.Title}, {book.Edition}, {book.Year}, {book.Quantity}, {author.AuthorId})";

                    int result = DB.CmdExecute(connection, addBook, transaction);

                    // Second, grab book that was just added to get the Id.
                    Book newBook = BookFinder(book.Title, book.Edition);
                    // Third, add Cover to Covers table
                    Cover newCover = new Cover()
                    {
                        BookId = newBook.BookId,
                        CoverImage = book.CoverImage
                    };
                    Covers covers = new Covers();
                    covers.AddCover(newCover);

                    // Fourth, add copies to a specific library
                    string addCopies = $"INSERT INTO Copies (BookId, LibraryId, NumberOfCopies) " +
                        $"VALUES ({newBook.BookId}, {book.LibraryId}, {book.NumberOfCopies})";

                    int copiesResult = DB.CmdExecute(connection, addCopies, transaction);

                    if (book.SubjectNames.Count <= 0) throw new Exception("There are no subjects for this book.");

                    foreach(string subject in book.SubjectNames)
                    {
                        Subject sbj = Subjects.SubjectFinder(subject);
                        string addSubjects = $"INSERT INTO BookSubject (BooksBookId, SubjectsSubjectId) " +
                        $"VALUES ({newBook.BookId}, {sbj.SubjectId})";

                        int subjectsResult = DB.CmdExecute(connection, addSubjects, transaction);
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public int UpdateBook(Book book)
        {
            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"UPDATE Books" +
                        $"SET Title = {book.Title}, Edition = {book.Edition}, Year = {book.Year}, Quantity = {book.Quantity}, AuthorId = {book.AuthorId}" +
                        $"WHERE BookId = {book.BookId}";

                    SqlTransaction transaction = connection.BeginTransaction();
                    int updatedRows = DB.CmdExecute(connection, query, transaction);

                    transaction.Commit();

                    return updatedRows;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public BooksInfo DeleteBookById(int id)
        {
            BooksInfo book = null;

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    List<BooksInfo> books = GetAllBooks(id);

                    if (books.Count != 1) throw new Exception("An error has occurred when trying to get the book.");

                    foreach(BooksInfo bk in books)
                    {
                        book = bk;
                    }

                    SqlTransaction transaction = connection.BeginTransaction();
                    // Delete from Orders
                    string deleteOrders = $"DELETE FOM Orders WHERE BookId = {id}";

                    int deletedOrders = DB.CmdExecute(connection, deleteOrders, transaction);

                    // Delete from copies
                    string deleteCopies = $"DELETE FOM Copies WHERE BookId = {id}";
                    int deletedCopies = DB.CmdExecute(connection, deleteCopies, transaction);

                    // Delete from Subjects
                    string deleteSubjects = $"DELETE FOM BookSubject WHERE BookId = {id}";
                    int deletedSubjects = DB.CmdExecute(connection, deleteSubjects, transaction);

                    // Delete from covers
                    string deleteCovers = $"DELETE FOM Covers WHERE BookId = {id}";
                    int deletedCovers = DB.CmdExecute(connection, deleteCovers, transaction);

                    // Delete from books
                    string deleteBook = $"DELETE FOM Books WHERE BookId = {id}";
                    int deletedBooks = DB.CmdExecute(connection, deleteBook, transaction);

                    transaction.Commit();
                    return book;
                }
            }
            catch(Exception e)
            {
                return book;
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public Book BookFinder(string title, string edition)
        {
            Book book = null;

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Books WHERE Books.Title = '{title}' AND Books.Edition = '{edition}'";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count == 0) return book = null;
                    
                    foreach(DataRow row in dataTable.Rows)
                    {
                        book = new Book() {
                            BookId = Convert.ToInt32(row["BookId"]),
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                            Title = row["Title"].ToString(),
                            Edition = row["Edition"].ToString(),
                            Year = Convert.ToInt32(row["Year"]),
                            Quantity = Convert.ToInt32(row["Quantity"])
                        };
                    }
                    
                    return book;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public List<BookSearchResult> GetBooksForSearch()
        {
            List<BookSearchResult> books = new List<BookSearchResult>();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = @"
                        SELECT 
                            Books.BookId,
                            Books.Title,
                            Books.Quantity,
                            Authors.AuthorId,
                            Authors.AuthorName,
                            STRING_AGG(Subjects.SubjectName, ', ') AS SubjectNames
                        FROM Books
                        INNER JOIN Authors ON Books.AuthorId = Authors.AuthorId
                        INNER JOIN BookSubject ON Books.BookId = BookSubject.BooksBookId
                        INNER JOIN Subjects ON BookSubject.SubjectsSubjectId = Subjects.SubjectId
                        GROUP BY 
                            Books.BookId,
                            Books.Title,
                            Books.Quantity,
                            Authors.AuthorId,
                            Authors.AuthorName
                        Order BY
                            Books.BookId";

                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var book = new BookSearchResult()
                        {
                            BookId = Convert.ToInt32(row["BookId"]),
                            Title = row["Title"].ToString(),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                            AuthorName = row["AuthorName"].ToString(),
                            SubjectNames = row["SubjectNames"].ToString().Split(',').Select(lst => lst.Trim()).ToList()
                        };
                        books.Add(book);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
            return books;

        }
    }
}
