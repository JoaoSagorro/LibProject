using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;
using static ADOLib.Model.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ADOLib
{
    public class Users
    {
        public string CnString { get; set; }

        public Users()
        {
            CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public User GetUserInfo(int id)
        {
            User user = null;
            
            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Users WHERE UserId = {id}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    foreach(DataRow row in dataTable.Rows)
                    {
                        user = new User
                        {
                            UserId = Convert.ToInt32(row["UserId"]),
                            RoleId = Convert.ToInt32(row["RoleId"]),
                            FirstName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Address = row["Address"].ToString(),
                            Email = row["Email"].ToString(),
                            Password = row["Password"].ToString(),
                            Birthdate = Convert.ToDateTime(row["Birthdate"]),
                            RegisterDate = Convert.ToDateTime(row["RegisterDate"]),
                            Suspended = Convert.ToBoolean(row["RegisterDate"]),
                            Active = Convert.ToBoolean(row["Active"]),
                            Strikes = Convert.ToInt32(row["Strikes"])
                        };
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return user;
        }

        public int StrikeUser(int orderId)
        {
                using(SqlConnection connection = DB.Open(CnString))
                {
                var transaction = connection.BeginTransaction();
                try
                {
                string query = $@"UPDATE u
                                SET u.Strikes = u.Strikes + 1
                                FROM Users u
                                INNER JOIN Orders o ON u.UserId = o.UserId
                                WHERE o.OrderId = {orderId}";
                int changed = DB.CmdExecute(connection, query, transaction);
                string getNewStrikes = $@"Select Strikes
                                        FROM User u
                                        INNER JOIN Orders o ON u.UserID = o.UserId
                                        WHERE o.OrderId = {orderId}";
                int strikes =int.Parse(DB.GetSQLRead(connection, query).Rows[0]["Strikes"].ToString());
                transaction.Commit();
                return strikes;
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new Exception($"Error adding strike to User: {e.Message}");
                }
                }
        }

    }
}
