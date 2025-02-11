﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.IdentityModel.Tokens;

namespace LibLibrary
{
    public class LibOrders
    {

        public List<Order> GetAllOrders()
        {
            List<Order> allOrders = new List<Order>();

            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    allOrders = context.Orders.Select(ord => ord).ToList();

                    if (allOrders.IsNullOrEmpty()) throw new Exception("There are still no orders that have been placed.");
                }
            }
            catch (Exception e)
            {
                throw e;
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
