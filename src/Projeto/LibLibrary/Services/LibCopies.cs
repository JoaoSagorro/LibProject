﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace LibLibrary.Services
{
    public class LibCopies
    {
        // this copie parameter has the number of copies TO ADD to the old copie object.
        public static void UpdateNumberOfCopies(Copie copie)
        {
            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    if(HasCopies(copie.BookId, copie.LibraryId))
                    {
                        Book book = LibBooks.GetBookById(copie.BookId);
                        Copie oldCopie = context.Copies.First<Copie>(cp => cp.BookId == copie.BookId && cp.LibraryId == copie.LibraryId);
                        if(oldCopie.NumberOfCopies + copie.NumberOfCopies < 1)
                        {
                            throw new Exception("Can't remove that many copies.");
                        }
                        oldCopie.NumberOfCopies += copie.NumberOfCopies;
                        book.Quantity += copie.NumberOfCopies;
                        context.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        
        public static Copie GetCopy(int bookId, int libraryId)
        {
            try
            {
                using var context = new LibraryContext();
                var copy = context.Copies.Include(c => c.Book).Include(c => c.Library).FirstOrDefault(c => c.LibraryId == libraryId && c.BookId == bookId);
                return copy;
             }
            catch(Exception e) { throw new Exception($"Error getting copy: {e.Message}", e); }
        }

        public static bool HasCopies(int bookId, int libraryId)
        {
            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    Copie? copie = context.Copies.FirstOrDefault(cp => cp.LibraryId == libraryId && cp.BookId == bookId);

                    return copie is not null;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public static int GetLibCount(int bookId)
        {
            using var context = new LibraryContext();
            return context.Copies.Where(cp => cp.BookId == bookId).Count();
        }

        public static void CreateInitialCopies(int bookId, int libraryId, int qty)
        {
            using var context = new LibraryContext();
            Copie newCopie = new Copie { BookId = bookId, LibraryId = libraryId, NumberOfCopies = qty };
            context.Copies.Add(newCopie);
            context.SaveChanges();
        }
    }
}
