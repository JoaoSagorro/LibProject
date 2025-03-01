using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibLibrary.Services
{
    public class LibOrders
    {

        public List<Order> CheckOrderState()
        {
            List<Order> checkedOrders = new List<Order>();

            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    var orders = context.Orders.Select(a => a);

                    foreach(Order order in orders)
                    {
                        int dayDiff = (DateTime.Now - order.OrderDate).Days;

                        if(dayDiff > 15)
                        {
                            order.State = context.States.First(a => a.StateName == "ATRASO");
                        }
                        else if(dayDiff > 12)
                        {
                            order.State = context.States.First(a => a.StateName == "Devolução URGENTE");
                        }
                        else if (dayDiff > 10)
                        {
                            order.State = context.States.First(a => a.StateName == "Devolução em breve");
                        }

                        checkedOrders.Add(order);
                    }

                    context.UpdateRange(checkedOrders);
                    context.SaveChanges();
                    return checkedOrders;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public List<Order> GetAllOrders()
        {
            List<Order> allOrders = new List<Order>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    allOrders = context
                        .Orders
                        .Include(bk => bk.Book)
                        .Include(user => user.User)
                        .Include(st => st.State)
                        .Include(lib => lib.Library)
                        .Select(ord => ord).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return allOrders;
        }

        public Order GetOrderById(int id)
        {
            Order order;

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    order = context.Orders.FirstOrDefault(ord => ord.OrderId == id, null);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return order;
        }


        public List<Order> GetOrderByUserId(int userId)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    var orderList = context.Orders.Where(ord => ord.User.UserId == userId);

                    orders.AddRange(orderList);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return orders;
        }

        public List<Order> GetOrdersByBook(int bookId)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    var bookOrders = context.Orders.Where(ord => ord.Book.BookId == bookId);

                    orders.AddRange(bookOrders);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return orders;
        }

        public List<Order> GetOrdersByLibrary(int libraryId)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    var libraryOrders = context.Orders.Where(ord => ord.Library.LibraryId == libraryId);

                    orders.AddRange(libraryOrders);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return orders;
        }

        public List<Order> GetOrdersByDate(DateTime orderDate)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    var ordersByDate = context.Orders.Where(ord => ord.OrderDate == orderDate);

                    orders.AddRange(ordersByDate);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return orders;
        }
    }
}
