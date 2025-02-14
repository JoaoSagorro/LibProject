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


        public static List<LibraryStats> GetLibraryWithLessOrders()
        {
            List<LibraryStats> ordByLib = new List<LibraryStats>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    ordByLib = context
                        .Orders
                        .Include(ord => ord.User)
                        .Include(ord => ord.Library)
                        .GroupBy(bk => bk.Library.LibraryName)
                        .Select(group => new LibraryStats
                        {
                            LibraryName = group.Key,
                            OrdersCount = group.Count(),
                            UsersCount = group.Select(user => user.User).Distinct().Count(),
                            OrderDate = group.Select(date => date.OrderDate).Max()
                        })
                        .OrderBy(ord => ord.OrdersCount)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return ordByLib;
        }

        public static List<LibraryStats> GetLibraryWithLessUsers()
        {
            List<LibraryStats> ordByUser = new List<LibraryStats>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    ordByUser = context
                        .Orders
                        .Include(ord => ord.User)
                        .Include(ord => ord.Library)
                        .GroupBy(bk => bk.Library.LibraryName)
                        .Select(group => new LibraryStats
                        {
                            LibraryName = group.Key,
                            OrdersCount = group.Count(),
                            UsersCount = group.Select(user => user.User).Distinct().Count(),
                            OrderDate = group.Select(date => date.OrderDate).Max()
                        })
                        .OrderBy(ord => ord.UsersCount)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return ordByUser;
        }


        public static List<LibraryStats> GetLibraryWithOldestRequest()
        {
            List<LibraryStats> ordByDate = new List<LibraryStats>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    ordByDate = context
                        .Orders
                        .Include(ord => ord.User)
                        .Include(ord => ord.Library)
                        .GroupBy(bk => bk.Library.LibraryName)
                        .Select(group => new LibraryStats
                        {
                            LibraryName = group.Key,
                            OrdersCount = group.Count(),
                            UsersCount = group.Select(user => user.User.UserId).Distinct().Count(),
                            OrderDate = group.Select(date => date.OrderDate).Min()
                        })
                        .OrderBy(ord => ord.OrderDate)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return ordByDate;
        }
    }
}
