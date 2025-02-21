using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class UserService
    {
        public string CnString { get; set; }

        public UserService()
        {
            CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProject;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public async Task<bool> RegisterUser(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CnString))
                {
                    await connection.OpenAsync();

                    string query = @"INSERT INTO Users (RoleId, FirstName, LastName, Address, Email, Password, Birthdate, RegisterDate, Suspended, Active, Strikes) 
                                     VALUES (@RoleId, @FirstName, @LastName, @Address, @Email, @Password, @Birthdate, @RegisterDate, @Suspended, @Active, @Strikes)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", user.LastName);
                        cmd.Parameters.AddWithValue("@Address", user.Address);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@Birthdate", user.Birthdate);
                        cmd.Parameters.AddWithValue("@RegisterDate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@Suspended", false);
                        cmd.Parameters.AddWithValue("@Active", true);
                        cmd.Parameters.AddWithValue("@Strikes", 0);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public bool EmailExists(string email)
        {
            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0; // Returns true if the email exists
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Database error while checking email existence", e);
            }
        }

    }
}

