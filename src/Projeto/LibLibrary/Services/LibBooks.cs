﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibLibrary.Services
{
    public class LibBooks
    {

        public static List<Book> GetAllBooks()
        {
            List<Book> allBooks;
            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    allBooks = context.Books.Select(bk => bk).ToList();

                    if (allBooks.IsNullOrEmpty())
                    {
                        throw new Exception("There are no books in our database");
                    }
                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message, e.InnerException);
            }

            return allBooks;
        }

        public static Book GetBookById(int bookId)
        {
            Book book;

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    book = context.Books.Include(b => b.Author).FirstOrDefault(bk => bk.BookId == bookId);

                    if (book is null) throw new Exception("The book does not exist.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return book;
        }

        public static void EditBook(Book book)
        {
            //book is the book with the changes already made
            try
            {
                using var context = new LibraryContext();
                var bookToEdit = GetBookById(book.BookId);
                bookToEdit.Author = book.Author;
                bookToEdit.Year = book.Year;
                bookToEdit.Title = book.Title;
                bookToEdit.Edition = book.Edition;
                context.Update(book);
                context.SaveChanges();

            }
            catch(Exception e) { throw new Exception("Error Editing book: \n", e); }
        }
        // Add book
        // Just for administrator
        // It's missing subject, cover and copies logic
        public static void AddBook(Book book)
        {
            // Author object
            // ICollection of Subjects
            // ICollection of Copies
            // Cover object
            Author bookAuthor = book.Author;
            Cover bookCover = book.Cover;

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    List<Subject> subjects = [];
                Author author = context.Authors.FirstOrDefault(b => b.AuthorName == book.Author.AuthorName);
                    if (author != null) book.Author = author;
                    else { bookAuthor = book.Author; }
                    // LibCover.AddCover(bookCover);
                    foreach(var sub in book.Subjects)
                    {
                        if(!LibSubjects.SubjectExists(sub, context))
                        {
                            LibSubjects.AddSubject(sub, context);
                        }
                        var subject = context.Subjects.FirstOrDefault(s => s.SubjectName == sub.SubjectName);
                        subjects.Add(subject);
                    }

                    book.Subjects = subjects;
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
                        LibAuthor.AddAuthor(bookAuthor);
                        context.Books.Add(book);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
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

        // Deleted Book
        // Still need to delete every regist of books -> when deleting book, delete from the other tables as well
        // By default EFCore has "On delete cascade"
        public static Book DeleteBookById(int bookId)
        {
            Book bookToDel = null;
            Book deleted = null;

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    bookToDel = context.Books.FirstOrDefault(b => b.BookId == bookId);
                    deleted = CopieBook(bookToDel);
                    context.Remove(bookToDel);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return deleted;
        }

        // Verification for empty camps of the Book object
        public static void UpdateBook(Book book)
        {
            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    context.Books.Update(book);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
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

        public static List<Copie> GetCopies(Book book)
        {
            try
            {
            using var context = new LibraryContext();
            var copieList = new List<Copie>();
            copieList.AddRange(context.Copies.Include(c => c.Library).Where(c => c.BookId == book.BookId).ToList());
            return copieList;
            }catch(Exception e) { throw new Exception("Error getting copies", e); }
        }

    }
}
