using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            CnString = "Server=host.docker.internal;Database=upskill_Projeto_EFJ;User Id = Nony; Password=1234Tukito69;Trusted_Connection=False;TrustServerCertificate=True";
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

    }
}
