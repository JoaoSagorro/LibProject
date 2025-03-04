using System.Data;
using LibDB;
using Microsoft.Data.SqlClient;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class Users
    {
        public string CnString { get; set; }

        public Users()
        {
            CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
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
                            Suspended = Convert.ToBoolean(row["Suspended"]),
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

        public int StrikeUser(int orderId, SqlTransaction transaction)
        {
            using (SqlConnection connection = DB.Open(CnString))
            {
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
                    int strikes = int.Parse(DB.GetSQLRead(connection, query).Rows[0]["Strikes"].ToString());
                    return strikes;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception($"Error adding strike to User: {e.Message}");
                }
            }
        }

        //public int StrikeUser(int orderId, SqlTransaction transaction, SqlConnection connection)
        //{
        //        try
        //        {
        //            string query = $@"UPDATE u
        //                        SET u.Strikes = u.Strikes + 1
        //                        FROM Users u
        //                        INNER JOIN Orders o ON u.UserId = o.UserId
        //                        WHERE o.OrderId = {orderId}";
        //            int changed = DB.CmdExecute(connection, query, transaction);
        //            string getNewStrikes = $@"Select Strikes
        //                                FROM User u
        //                                INNER JOIN Orders o ON u.UserID = o.UserId
        //                                WHERE o.OrderId = {orderId}";
        //            int strikes = int.Parse(DB.GetSQLRead(connection, query,transaction).Rows[0]["Strikes"].ToString());
        //            return strikes;
        //        }
        //        catch (Exception e)
        //        {
        //            transaction.Rollback();
        //            throw new Exception($"Error adding strike to User: {e.Message}");
        //        }
        //}

        public int StrikeUser(int orderId, SqlTransaction transaction, SqlConnection connection)
        {
            try
            {
                // Define the query to increment strikes and return the new strikes count
                string query = @"
            UPDATE Users
            SET Strikes = Strikes + 1
            OUTPUT INSERTED.Strikes
            FROM Users u
            INNER JOIN Orders o ON u.UserId = o.UserId
            WHERE o.OrderId = @OrderId";

                // Initialize the command with the query and connection
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    // Add the parameter to the command
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    // Execute the command and retrieve the new strikes count
                    object result = command.ExecuteScalar();

                    // Check if the result is not null and return the strikes count
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        // Handle the case where no rows were affected
                        throw new Exception("No matching order found for the provided OrderId.");
                    }
                }
            }
            catch (Exception e)
            {
                // Rollback the transaction in case of an error
                transaction.Rollback();
                throw new Exception($"Error adding strike to User: {e.Message}", e);
            }
        }
    }
}
