using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace LibLibrary.Services
{
    public class LibStatistics
    {

        public static Dictionary<ICollection<Subject>, int> GetMostBoughtSubjects()
        {
            Dictionary<ICollection<Subject>, int> bookList = new Dictionary<ICollection<Subject>, int>();

            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    var list = context
                        .Orders
                        .GroupBy(ord => ord.Book.Subjects)
                        .Select(group => new
                        {
                            Subject = group.Key,
                            Count = group.Count(),
                        })
                        .OrderByDescending(sbj => sbj.Count).ToList();

                    foreach (var obj in list)
                    {
                        var subject = obj.Subject;
                        int quantity = obj.Count;

                        bookList.Add(subject, quantity);
                    }
                }
            }
            catch (Exception e )
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return bookList;
        }
        
        public static Dictionary<Dictionary<List<Book>, List<Library>>, int> GetBooksInLibraries()
        {
            Dictionary<Dictionary<List<Book>, List<Library>>, int> dictionary = new Dictionary<Dictionary<List<Book>, List<Library>>, int>();
            Dictionary<List<Book>, List<Library>> lastDic = new Dictionary<List<Book>, List<Library>>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    var list = context
                        .Copies
                        .Include(lib => lib.Library)
                        .Include(bk => bk.Book)
                        .GroupBy(bk => new { bk.Book, bk.Library })
                        .Select(group => new
                        {
                            Books = group.Key.Book,
                            Libraries = group.Key.Library,
                            Quantities = group.Select(e => e.NumberOfCopies)
                        }).ToList();

                    foreach(var l in list)
                    {
                        Book book = l.Books;
                        Library lib = l.Libraries;
                        if(l.Quantities.Count() != 1) 
                        { 
                            throw new Exception($"It wasn't possible to get the copies of {l.Books.Title} in the {l.Libraries.LibraryName}."); 
                        }
                        int quantity = l.Quantities.First();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

    }
}
