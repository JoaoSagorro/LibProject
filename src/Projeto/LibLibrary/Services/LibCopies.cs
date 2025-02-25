using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;

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
    }
}
