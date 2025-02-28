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
            //CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProject;Trusted_Connection=True;TrustServerCertificate=True";
        CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
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

        public async Task<User?> LoginUser(string email, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CnString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new User
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Email = reader.GetString(reader.GetOrdinal("Email"))
                                };
                            }
                            else
                            {
                                // Throw a specific exception for invalid credentials
                                throw new UnauthorizedAccessException("Invalid email or password.");
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Rethrow the specific exception for invalid credentials
                throw;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow a generic exception
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error occurred while logging in.");
            }
        }

    }
}

