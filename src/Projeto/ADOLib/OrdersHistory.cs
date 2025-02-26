﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class OrdersHistory
    {
        private string CnString { get; set; }

        public OrdersHistory()
        {
            CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public static void AddHistory(string cnString, OrderHistory history)
        {
            try
            {
                using (SqlConnection connection = DB.Open(cnString))
                {
                    string query = $"INSERT INTO OrderHistories (UserName, BookName, BookYear, " +
                        $"BookEdition, BookAuthor, LibraryName, " +
                        $"OrderedCopies, OrderDate, ReturnDate) " +
                        $"VALUES ('{history.UserName}', '{history.BookName}', {history.BookYear}, " +
                        $"'{history.BookEdition}', '{history.BookAuthor}', '{history.LibraryName}', " +
                        $"{history.OrderedCopies}, {history.OrderDate}, {history.ReturnDate})";

                    SqlTransaction transaction = connection.BeginTransaction();

                    int rowsAffected = DB.CmdExecute(connection, query, transaction);
                    transaction.Commit();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

    }
}
