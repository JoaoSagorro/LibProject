using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using LibLibrary.AuthorServices;
using Microsoft.IdentityModel.Tokens;

namespace LibLibrary.BookServices
{
    public class LibBooks
    {

        public static List<Book> GetAllBooks()
        {
            List<Book> allBooks;
            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    allBooks = context.Books.Select(bk => bk).ToList();

                    if(allBooks.IsNullOrEmpty())
                    {
                        throw new Exception("There are no books in our database");
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }

            return allBooks;
        }

        public static Book GetBookById(int bookId)
        {
            Book book;

            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    book = context.Books.FirstOrDefault(bk => bk.BookId == bookId, null);

                    if (book is null) throw new Exception("The book does not exist.");
                }
            }
            catch(Exception e)
            {
                throw e;
            }

            return book;
        }

        // Add book
        // Just for admins
        // It's missing subject, cover and copies logic
        public static void AddBook(Book book)
        {
            // Author object
            // ICollection of Subjects
            // ICollection of Copies
            // Cover object
            Author bookAuthor = book.Author;
            List<Subject> bookSubject = book.Subjects.ToList();
            List<Copie> bookCopies = book.Copies.ToList();
            Cover bookCover = book.Cover;

            try
            {
                using (LibraryContext context = new LibraryContext())
                {

                    if (BookExists(book.Title, book.Edition))
                    {
                        throw new Exception("The book already exists.");
                    }

                    if (LibAuthor.AuthorExists(bookAuthor.AuthorName))
                    {
                        context.Books.Add(book);
                        context.SaveChanges();
                    }

                    if (!LibAuthor.AuthorExists(bookAuthor.AuthorName))
                    {
                        Author newAuthor = LibAuthor.AddAuthor(bookAuthor);
                        context.Books.Add(book);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Book CopieBook(Book book)
        {
            Book newBook = new Book
            {
                BookId = book.BookId,
                Title = book.Title,
                Edition = book.Edition,
                Year = book.Year,
                Quantity = book.Quantity,
                Author = book.Author,
                Cover = book.Cover,
                Subjects = book.Subjects,
                Copies = book.Copies
            };

            return newBook;
        }

        // Eliminar obra
        // Rever verificações e o que retornar nesta função
        // Só para administradores
        public Book DeleteBookById(int bookId)
        {
            Book bookToDel;
            Book deleted;
            using (LibraryContext context = new LibraryContext())
            {
                bookToDel = context.Books.First(b => b.BookId == bookId);

                deleted = CopieBook(bookToDel);

                context.Remove(bookToDel);
                context.SaveChanges();
            }

            return deleted;
        }

        // Método para fazer update da obra
        // MÉTODO AINDA NÃO ESTÁ COMPLETO
        // Só para administradores
        public static (bool success, string message) UpdateBook
            (string title,
            string edition,
            int year,
            int quantity,
            string author)
        {
            bool success = false;
            string message = "";

            using (LibraryContext context = new LibraryContext())
            {
                Book book = BookFinder(title);

                book.Title = title;
                book.Edition = edition;
                book.Year = year;
                book.Quantity = quantity;

            }

            return (success, message);
        }

        private static bool BookExists(string book, string edition)
        {
            bool success = false;

            using (LibraryContext context = new LibraryContext())
            {
                foreach (Book b in context.Books)
                {
                    if (b.Title == book && b.Edition == edition)
                    {
                        success = true;
                    }
                }

                return success;
            }
        }

        public static Book BookFinder(string t)
        {
            using (LibraryContext context = new LibraryContext())
            {
                var query = context.Books.First(a => a.Title == t);
                Book book = query;

                return book;
            }
        }

        public static List<Subject> SubjectFinder(List<string> s)
        {
            using (LibraryContext context = new LibraryContext())
            {
                List<Subject> lista = new List<Subject>();

                foreach (string sbj in s)
                {
                    var query = context.Subjects.First(name => name.SubjectName == sbj);
                    Subject subject = query;
                    lista.Add(subject);
                }

                return lista;
            }
        }

    }
}
