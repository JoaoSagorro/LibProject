﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOLib.Enums;
using ADOLib.ModelView;
using EFLibrary;
using LibDB;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class Orders
    {
        private string CnString { get; set; }

        public Orders()
        {
            //CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
            CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        public List<UserOrder> CheckOrderState(int userId)
        {
            List<UserOrder> orders = new List<UserOrder>();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = @"
                        SELECT 
                            Orders.OrderId,
                            Books.Title,
                            Authors.AuthorName,
                            Libraries.LibraryName,
                            Orders.OrderDate,
                            Orders.ReturnDate,
                            States.StateName
                        FROM Orders
                        INNER JOIN Books ON Orders.BookId = Books.BookId
                        INNER JOIN Authors ON Books.AuthorId = Authors.AuthorId
                        INNER JOIN Libraries ON Orders.LibraryId = Libraries.LibraryId
                        INNER JOIN States ON Orders.StateId = States.StateId
                        WHERE Orders.UserId = " + userId.ToString() + @"
                        GROUP BY 
                            Orders.OrderId,
                            Books.Title,
                            Authors.AuthorName,
                            Libraries.LibraryName,
                            Orders.OrderDate,
                            Orders.ReturnDate,
                            States.StateName
                        Order BY
                            Orders.OrderDate";

                    string updateQuery = $"UPDATE Orders SET StateId = @stateId WHERE OrderId = @orderId";
                    DataTable dataTable = DB.GetSQLRead(connection, query);
                    States states = new States();

                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        foreach (DataRow order in dataTable.Rows)
                        {
                            int dayDiff = (DateTime.Now - Convert.ToDateTime(order["OrderDate"])).Days;

                            int id = Convert.ToInt32(order["OrderId"]);
                            cmd.Parameters.AddWithValue("@orderId", id);
                            int stateId = (int)StatesEnum.Solicitado;

                            if (order["ReturnDate"] != DBNull.Value)
                            {
                                stateId = (int)StatesEnum.Devolvido;
                            }
                            else if (dayDiff > 15)
                            {
                                stateId = (int)StatesEnum.EmAtraso;
                            }
                            else if (dayDiff > 12)
                            {
                                stateId = (int)StatesEnum.DevolucaoUrgente;
                            }
                            else if (dayDiff > 10)
                            {
                                stateId = (int)StatesEnum.DevolucaoEmBreve;
                            }

                            UserOrder newOrder = new UserOrder()
                            {
                                OrderId = Convert.ToInt32(order["OrderId"]),
                                Title = order["Title"].ToString(),
                                AuthorName = order["AuthorName"].ToString(),
                                LibraryName = order["LibraryName"].ToString(),
                                OrderDate = Convert.ToDateTime(order["OrderDate"]),
                                ReturnDate = order["ReturnDate"] != DBNull.Value ? Convert.ToDateTime(order["ReturnDate"]) : DateTime.MinValue,
                                StateName = states.GetStateById(stateId).StateName
                            };

                            orders.Add(newOrder);

                            cmd.Parameters.AddWithValue("@stateId", stateId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return orders;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public Order GetOrderById(int orderId)
        {
            Order order = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Orders WHERE OrderId = {orderId}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) return order;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        order = new Order()
                        {
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            BookId = Convert.ToInt32(row["BookId"]),
                            UserId = Convert.ToInt32(row["UserId"]),
                            LibraryId = Convert.ToInt32(row["LibraryId"]),
                            StateId = Convert.ToInt32(row["StateId"]),
                            OrderDate = Convert.ToDateTime(row["OrderDate"]),
                            ReturnDate = Convert.ToDateTime(row["ReturnDate"]),
                        };
                    }
                }
            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return order;
        } 

        public List<Order> GetOrdersByUserId(int userId)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Orders WHERE UserId = {userId}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count == 0) return orders;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Order order = new Order()
                        {
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            BookId = Convert.ToInt32(row["BookId"]),
                            UserId = Convert.ToInt32(row["UserId"]),
                            LibraryId = Convert.ToInt32(row["LibraryId"]),
                            StateId = Convert.ToInt32(row["StateId"]),
                            OrderDate = Convert.ToDateTime(row["OrderDate"]),
                            ReturnDate = Convert.ToDateTime(row["ReturnDate"]),
                        };

                        orders.Add(order);
                    }
                }
            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return orders;
        }


        public List<Order> DeleteUserOrders(int userId)
        {
            List<Order> orders = null;

            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    orders = GetOrdersByUserId(userId);
                    string deleteOrders = "DELETE FROM Orders WHERE Orders.UserId = @userId";
                    SqlTransaction transaction = connection.BeginTransaction();
                    using(SqlCommand cmd = new SqlCommand(deleteOrders, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        int affectedRows = cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return orders;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
