using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;

namespace LibLibrary
{
    public class LibOrders
    {

        public List<Order> GetAllOrders()
        {
            List<Order> allOrders;

            using(LibraryContext context = new LibraryContext())
            {
                allOrders = context.Orders.Select(ord => ord).ToList();
            }

            return allOrders;
        }

        public Order GetOrderById(int id)
        {
            Order order;

            using(LibraryContext context = new LibraryContext())
            {
                order = context.Orders.First(ord => ord.OrderId == id);
            }

            return order;
        }


        public List<Order> GetOrderByUserId(int userId)
        {
            List<Order> orders;

            using(LibraryContext context = new LibraryContext())
            {
                orders = context.Orders.Where(ord => ord.User.UserId == userId).ToList();
            }

            return orders;
        }
    }
}
