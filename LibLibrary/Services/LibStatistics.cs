using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using EFLibrary.ModelView;
using Microsoft.EntityFrameworkCore;

namespace LibLibrary.Services
{
    public class LibStatistics
    {       
        public static List<SubjectStats> GetMostRequestedSubjects()
        {
            List<SubjectStats> sbjStsList = new List<SubjectStats>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    // Need a query that returns the Subject Name and the Count of books requested with that Subject
                    sbjStsList = context
                        .Orders
                        .Include(obj => obj.Book)
                        .ThenInclude(ob => ob.Subjects)
                        .SelectMany(slct => slct.Book.Subjects, (order, subject) => new {order, subject})
                        .GroupBy(group => group.subject.SubjectName)
                        .Select(sbj => new SubjectStats { Subjects = sbj.Key, SubjectsCount = sbj.Count() })
                        .OrderByDescending(count => count.SubjectsCount)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return sbjStsList;
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
