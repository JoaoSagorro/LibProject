using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class Covers
    {
        private string CnString { get; set; }

        public Covers()
        {
            CnString = "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public void AddCover(int bookId, byte[] image)
        {
            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = "INSERT INTO Covers (CoverId, CoverImage) VALUES (@coverId, @image)";
                    SqlTransaction transaction = connection.BeginTransaction();

                    using(SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@coverId", bookId);
                        cmd.Parameters.AddWithValue("@image", image);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public int UpdateCover(Cover cover)
        {
            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"UPDATE Covers SET CoverImage = @image WHERE CoverId = @coverId";
                    SqlTransaction transaction = connection.BeginTransaction();

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@coverId", cover.BookId);
                        cmd.Parameters.AddWithValue("@image", cover.CoverImage);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        transaction.Commit();
                        return rowsAffected;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public Cover DeleteCoverById(int id)
        {
            Cover delCover = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    delCover = GetCoverById(id);
                    string deleteQuery = "DELETE FROM Covers WHERE CoverId = @coverId";
                    SqlTransaction transaction = connection.BeginTransaction();

                    using(SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@coverId", id);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    return delCover;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public static async Task<byte[]> ConvertFileToImage(string filePath)
        {
            byte[] image = null;

            try
            {
                Uri uriResult;
                bool isUrl = Uri.TryCreate(filePath, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (isUrl && uriResult is not null)
                {
                    using (HttpClient webClient = new HttpClient())
                    {
                        image = await webClient.GetByteArrayAsync(filePath);

                        if (image == null) throw new Exception("Unable to convert image.");
                    }
                }

                if (!isUrl)
                {
                    image = await File.ReadAllBytesAsync(filePath);
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return image;
        }

        public Cover GetCoverById(int id)
        {
            Cover cover = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Covers WHERE AuthorId = {id}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) throw new Exception("An error has occurred when trying to find the author.");

                    foreach (DataRow row in dataTable.Rows)
                    {
                        cover = new Cover()
                        {
                            BookId = Convert.ToInt32(row["CoverId"]),
                            CoverImage = (byte[])row["CoverImage"],
                        };
                    }
                }

            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return cover;
        }
    }
}
