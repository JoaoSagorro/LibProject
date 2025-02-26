using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class Copies
    {
        private static string CnString { get; set; }

        public Copies()
        {
            CnString = "Server=DESKTOP-JV2HGSK;Database=LibraryProjectV2;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public Copie GetCopies(int bookId, int libId)
        {
            Copie copie = null;
            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Copies WHERE BookId = {bookId} AND LibraryId = {libId}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) return copie;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        copie = new Copie()
                        {
                            BookId = Convert.ToInt32(row["BookId"]),
                            LibraryId = Convert.ToInt32(row["LibraryId"]),
                            NumberOfCopies = Convert.ToInt32(row["NumberOfCopies"])
                        };
                    }
                }

                return copie;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public static void ChangeNumberOfCopies(Copie copie, int orderedCopies)
        {
            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    int remaining = Convert.ToInt32(copie.NumberOfCopies) - orderedCopies;
                    string updateCopies = "UPDATE Copies SET NumberOfCopies = @remaining WHERE BookId = @bookId AND LibraryId = @libraryId";
                    SqlTransaction transaction = connection.BeginTransaction();

                    using(SqlCommand cmd = new SqlCommand(updateCopies, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@remaining", remaining);
                        cmd.Parameters.AddWithValue("@bookId", Convert.ToInt32(copie.BookId));
                        cmd.Parameters.AddWithValue("@libraryId", Convert.ToInt32(copie.LibraryId));

                        cmd.ExecuteNonQuery();
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
