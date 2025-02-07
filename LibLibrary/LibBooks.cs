using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;

namespace LibLibrary
{
    public class LibBooks
    {

        // Adicionar obra
        // Só para administradores
        // Falta subject e cover (na tabela assuntos_obras) e copies (na tabela exemplares) 
        public static Book AddBook(Book newBook)
        {
            Author bookAuthor = newBook.Author;
            List<Subject> bookSubject = newBook.Subjects.ToList();
            List<Copie> bookCopies = newBook.Copies.ToList();
            Cover bookCover = newBook.Cover;

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    if (BookExists(newBook.Title))
                    {
                        throw new Exception("The book already exists.");
                    }

                    if (LibAuthor.AuthorExists(bookAuthor.AuthorName) && LibAuthor.AuthorFinder(bookAuthor.AuthorName) != null)
                    {
                        if (!BookExists(newBook.Title))
                        {
                            context.Books.Add(newBook);
                            context.SaveChanges();
                        }
                    }

                    if (!LibAuthor.AuthorExists(bookAuthor.AuthorName))
                    {
                        if (!BookExists(newBook.Title))
                        {
                            Author newAuthor = LibAuthor.AddAuthor(bookAuthor.AuthorName);

                            context.Books.Add(newBook);
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return newBook;
        }

        public static Book Copie(Book book)
        {
            Book newBook = new Book
            {
                BookId = book.BookId,
                Title = book.Title,
                Year = book.Year,

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

                deleted = Copie(bookToDel);

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


        


        

        private static bool BookExists(string s)
        {
            bool success = false;

            using (LibraryContext context = new LibraryContext())
            {
                foreach (Book b in context.Books)
                {
                    if (b.Title == s)
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
