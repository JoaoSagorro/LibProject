using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            CnString = "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";
        }


        private string QueryCreator(int? id = null)
        {
            string query = "";
            if(id is null)
            {
                query = "SELECT Books.BookId, " +
                        "Books.title, Books.Edition, Books.Year, Books.Quantity, " +
                        "Authors.AuthorId, Authors.AuthorName, " +
                        "Libraries.LibraryId, Libraries.LibraryName, Libraries.LibraryAddress, Libraries.Email, Libraries.Contact, " +
                        "Copies.NumberOfCopies, Covers.CoverImage, " +
                        "STRING_AGG(Subjects.SubjectName, ', ') AS SubjectNames " +
                        "FROM Books " +
                        "INNER JOIN Copies ON Books.BookId = Copies.BookId " +
                        "INNER JOIN Libraries ON Copies.LibraryId = Libraries.LibraryId " +
                        "INNER JOIN Authors ON Books.AuthorId = Authors.AuthorId " +
                        "INNER JOIN BookSubject ON Books.BookId = BookSubject.BooksBookId " +
                        "INNER JOIN Covers ON Books.BookId = Covers.CoverId " +
                        "INNER JOIN Subjects ON BookSubject.SubjectsSubjectId = Subjects.SubjectId " +
                        "GROUP BY Books.BookId, Books.Title, Books.Edition, Books.Year, Books.Quantity, " +
                        "Authors.AuthorId, Authors.AuthorName, " +
                        "Libraries.LibraryId, Libraries.LibraryName, Libraries.LibraryAddress, Libraries.Email, Libraries.Contact, " +
                        "Copies.NumberOfCopies, Covers.CoverImage";
            }

            if(id is not null)
            {
                query = "SELECT Books.BookId, " +
                        "Books.title, Books.Edition, Books.Year, Books.Quantity, " +
                        "Authors.AuthorId, Authors.AuthorName, " +
                        "Libraries.LibraryId, Libraries.LibraryName, Libraries.LibraryAddress, Libraries.Email, Libraries.Contact, " +
                        "Copies.NumberOfCopies, Covers.CoverImage, " +
                        "STRING_AGG(Subjects.SubjectName, ', ') AS SubjectNames " +
                        "FROM Books " +
                        "INNER JOIN Copies ON Books.BookId = Copies.BookId " +
                        "INNER JOIN Libraries ON Copies.LibraryId = Libraries.LibraryId " +
                        "INNER JOIN Authors ON Books.AuthorId = Authors.AuthorId " +
                        "INNER JOIN BookSubject ON Books.BookId = BookSubject.BooksBookId " +
                        "INNER JOIN Covers ON Books.BookId = Covers.CoverId " +
                        "INNER JOIN Subjects ON BookSubject.SubjectsSubjectId = Subjects.SubjectId " +
                        $"WHERE Books.BookId = {id} " +
                        "GROUP BY Books.BookId, Books.Title, Books.Edition, Books.Year, Books.Quantity, " +
                        "Authors.AuthorId, Authors.AuthorName, " +
                        "Libraries.LibraryId, Libraries.LibraryName, Libraries.LibraryAddress, Libraries.Email, Libraries.Contact, " +
                        "Copies.NumberOfCopies, Covers.CoverImage";
            }

            return query;
        }

        public List<BooksInfo> GetAllBooks(int? id = null)
        {
            List<BooksInfo> books = new List<BooksInfo>();

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = QueryCreator(id);

                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    foreach(DataRow row in dataTable.Rows)
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
                            // Need to review this conversion and check if it isn't better just to create a converting method
                            CoverImage = (byte[])row["CoverImage"],
                            SubjectNames = row["SubjectName"].ToString().Split(",").Select(lst => lst.Trim()).ToList(),
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

        //public BooksInfo GetBookById(int id)
        //{
        //    BooksInfo book = null;

        //    try
        //    {
        //        using(SqlConnection context = DB.Open(CnString))
        //        {
        //            string query = $"SELECT * FROM Books WHERE BookId = {id}";
        //            DataTable dataTable = DB.GetSQLRead(context, query);

        //            foreach(DataRow row in dataTable.Rows)
        //            {
        //                book = new BooksInfo()
        //                {
        //                    BookId = Convert.ToInt32(row["BookId"]),
        //                    Title = row["Title"].ToString(),
        //                    Edition = row["Edition"].ToString(),
        //                    Year = Convert.ToInt32(row["Year"]),
        //                    Quantity = Convert.ToInt32(row["Quantity"]),
        //                    AuthorId = Convert.ToInt32(row["AuthorId"]),
        //                    LibraryId = Convert.ToInt32(row["LibraryId"]),
        //                    LibraryName = row["LibraryName"].ToString(),
        //                    LibraryAddress = row["LibraryAddress"].ToString(),
        //                    Email = row["Email"].ToString(),
        //                    Contact = row["Contact"].ToString(),
        //                    NumberOfCopies = Convert.ToInt32(row["NumberOfCopies"]),
        //                    AuthorName = row["AuthorName"].ToString(),
        //                    // Need to review this conversion and check if it isn't better just to create a converting method
        //                    CoverImage = (byte[])row["CoverImage"],
        //                    SubjectNames = row["SubjectName"].ToString().Split(",").Select(lst => lst.Trim()).ToList()
        //                };
        //            }

        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        throw new Exception(e.Message, e.InnerException);
        //    }

        //    return book;
        //}


        //public List<BooksWithSubjects> GetBooksWithSubjects()
        //{
        //    List<BooksWithSubjects> bookSubjects = new List<BooksWithSubjects>();

        //    try
        //    {
        //        using (SqlConnection connection = DB.Open(CnString))
        //        {
        //            string query = "SELECT Books.*, Subjects.* " +
        //                "FROM Books " +
        //                "INNER JOIN BookSubject ON Books.BookId = BookSubject.BooksBookId " +
        //                "INNER JOIN Subjects ON BookSubject.SubjectsSubjectId = Subjects.SubjectId";
        //            DataTable dataTable = DB.GetSQLRead(connection, query);

        //            foreach(DataRow row in dataTable.Rows)
        //            {
        //                BooksWithSubjects books = new BooksWithSubjects()
        //                {
        //                    BookId = Convert.ToInt32(row["BookId"]),
        //                    Title = row["Title"].ToString(),
        //                    Edition = row["Edition"].ToString(),
        //                    Year = Convert.ToInt32(row["Year"]),
        //                    Quantity = Convert.ToInt32(row["Quantity"]),
        //                    AuthorId = Convert.ToInt32(row["AuthorId"]),
        //                    SubjectId = Convert.ToInt32(row["SubjectId"]),
        //                    SubjectName = row["SubjectName"].ToString()
        //                };

        //                bookSubjects.Add(books);
        //            }
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        throw new Exception(e.Message, e.InnerException);
        //    }

        //    return bookSubjects;
        //}

        //public List<BooksByLibrary> GetBooksByLibrary()
        //{
        //    List<BooksByLibrary> books = new List<BooksByLibrary>();

        //    try
        //    {
        //        using(SqlConnection connection = DB.Open(CnString))
        //        {
        //            string query = "SELECT Books.*, Libraries.*, Copies.NumberOfCopies, Authors.AuthorName " +
        //                "FROM Books " +
        //                "INNER JOIN Copies ON Books.BookId = Copies.BookId " +
        //                "INNER JOIN Libraries ON Copies.LibraryId = Libraries.LibraryId " +
        //                "INNER JOIN Authors ON Books.AuthorId = Authors.AuthorId";

        //            DataTable dataTable = DB.GetSQLRead(connection, query);


        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message, e.InnerException);
        //    }
        //}
    }
}
