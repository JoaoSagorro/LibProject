using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOLib.DTOs;
using LibDB;
using Microsoft.Data.SqlClient;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class Libraries
    {
        private string CnString { get; set; }

        public Libraries()
        {
            CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        public Library GetLibraryById(int id)
        {
            Library lib = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Libraries WHERE LibraryId = {id}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) throw new Exception("An error has occurred when trying to find the library.");

                    foreach (DataRow row in dataTable.Rows)
                    {
                        lib = new Library()
                        {
                            LibraryId = Convert.ToInt32(row["LibraryId"]),
                            LibraryName = row["LibraryName"].ToString(),
                            LibraryAddress = row["LibraryAddress"].ToString(),
                            Email = row["Email"].ToString(),
                            Contact = row["Contact"].ToString()
                        };
                    }
                }

            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return lib;
        }

        public List<LibraryByNumberOfCopiesDTO> GetLibrariesByNumberOfCopies(int bookId)
        {
            List<LibraryByNumberOfCopiesDTO> libraries = new List<LibraryByNumberOfCopiesDTO>();

            // Validate bookId
            if (bookId <= 0)
            {
                return libraries; // Return empty list for invalid bookId
            }

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = @"
                SELECT 
                    Libraries.LibraryId, 
                    Libraries.LibraryName, 
                    Copies.NumberOfCopies
                FROM Libraries
                INNER JOIN Copies ON Libraries.LibraryId = Copies.LibraryId
                WHERE Copies.BookId = @BookId AND Copies.NumberOfCopies > 0;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BookId", bookId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LibraryByNumberOfCopiesDTO lib = new LibraryByNumberOfCopiesDTO()
                                {
                                    LibraryId = Convert.ToInt32(reader["LibraryId"]),
                                    LibraryName = reader["LibraryName"].ToString(),
                                    NumberOfCopies = Convert.ToInt32(reader["NumberOfCopies"]),
                                };

                                libraries.Add(lib);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Log the error (optional)
                Console.Error.WriteLine($"Error fetching libraries by number of copies: {e.Message}");
                throw new Exception("An error occurred while fetching libraries.", e);
            }

            return libraries;
        }
    }

}
    
