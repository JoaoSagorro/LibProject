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

        public List<Book> GetAllBooks()
        {
            DataTable dataTable = null;

            List<Book> books = new List<Book>();

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = "SELECT * FROM Books";

                    dataTable = DB.GetSQLRead(connection, query);

                    foreach(DataRow row in dataTable.Rows)
                    {
                        var book = new Book()
                        {
                            BookId = Convert.ToInt32(row["BookId"]),
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                            Title = row["Title"].ToString(),
                            Edition = row["Edition"].ToString(),
                            Year = Convert.ToInt32(row["Year"]),
                            Quantity = Convert.ToInt32(row["Quantity"])
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

        public Book GetBookById(int id)
        {
            Book book = null;

            try
            {
                using(SqlConnection context = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Books WHERE BookId = {id}";
                    DataTable dataTable = DB.GetSQLRead(context, query);

                    foreach(DataRow row in dataTable.Rows)
                    {
                        book = new Book()
                        {
                            BookId = Convert.ToInt32(row["BookId"]),
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                            Title = row["Title"].ToString(),
                            Edition = row["Edition"].ToString(),
                            Year = Convert.ToInt32(row["Year"]),
                            Quantity = Convert.ToInt32(row["Quantity"])
                        };
                    }

                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return book;
        }


        public List<BooksWithSubjects> GetBooksWithSubjects()
        {
            List<BooksWithSubjects> bookSubjects = new List<BooksWithSubjects>();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = "SELECT Books.*, Subjects.* " +
                        "FROM Books " +
                        "INNER JOIN BookSubject ON Books.BookId = BookSubject.BooksBookId " +
                        "INNER JOIN Subjects ON BookSubject.SubjectsSubjectId = Subjects.SubjectId";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    foreach(DataRow row in dataTable.Rows)
                    {
                        BooksWithSubjects books = new BooksWithSubjects()
                        {
                            BookId = Convert.ToInt32(row["BookId"]),
                            Title = row["Title"].ToString(),
                            Edition = row["Edition"].ToString(),
                            Year = Convert.ToInt32(row["Year"]),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                            SubjectId = Convert.ToInt32(row["SubjectId"]),
                            SubjectName = row["SubjectName"].ToString()
                        };

                        bookSubjects.Add(books);
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return bookSubjects;
        }
    }
}
