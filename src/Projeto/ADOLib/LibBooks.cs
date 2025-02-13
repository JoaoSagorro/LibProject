using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;

namespace ADOLib
{
    public class LibBooks
    {
        private string CnString { get; set; }

        public LibBooks()
        {
            CnString = "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public List<LibBooks> GetAllBooks()
        {
            DataTable dataTable = null;

            List<LibBooks> books = new List<LibBooks>();

            try
            {
                using(SqlConnection connection = new SqlConnection())
                {
                    string query = "SELECT * FROM Books";

                    dataTable = DB.GetSQLRead(connection, query);

                    foreach(DataRow row in dataTable.Rows)
                    {
                        var book = new LibBooks()
                        {
                            BookId = row["BookId"]
                        };
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

    }
}
