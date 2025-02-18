using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class Authors
    {
        private string CnString { get; set; }

        public Authors()
        {
            CnString = "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public List<Author> GetAllAuthors()
        {
            List<Author> author = new List<Author>();

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Authors";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count <= 0) throw new Exception("Está vazia");

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Author at = new Author()
                        {
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                            AuthorName = row["AuthorName"].ToString(),
                        };

                        author.Add(at);
                    }
                }

            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return author;
        }

        public Author GetAuthorById(int authorId)
        {
            Author author = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Authors WHERE AuthorId = {authorId}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) throw new Exception("An error has occurred when trying to find the author.");

                    foreach(DataRow row in dataTable.Rows)
                    {
                        author = new Author()
                        {
                            AuthorId = Convert.ToInt32(row["AuthorId"]),
                            AuthorName = row["AuthorName"].ToString(),
                        };
                    }
                }

            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return author;
        }

        // Just for admins
        public int AddAuthor(Author author)
        {
            try
            {
                // check if this verification makes sense.
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"INSERT INTO Authors (AuthorName) VALUES ('{author.AuthorName}')";

                    SqlTransaction transaction = connection.BeginTransaction();

                    int rowsAffected = DB.CmdExecute(connection, query, transaction);
                    transaction.Commit();

                    return rowsAffected;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        // Author parameter must contain the id of the author to be updated and the new values;
        public int UpdateAuthor(Author author)
        {
            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"UPDATE Authors SET AuthorName = '{author.AuthorName}' WHERE AuthorId = {author.AuthorId}";
                    SqlTransaction transaction = connection.BeginTransaction();

                    int rowsAffected = DB.CmdExecute(connection, query, transaction);

                    transaction.Commit();
                    return rowsAffected;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public Author DeleteAuthorById(int id)
        {
            Author delAuthor = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    delAuthor = GetAuthorById(id);
                    string deleteQuery = $"DELETE FROM Authors WHERE AuthorId = {id}";
                    SqlTransaction transaction = connection.BeginTransaction();

                    int rowsAffected = DB.CmdExecute(connection, deleteQuery, transaction);
                    transaction.Commit();

                    return delAuthor;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }


        }

        // needs to be reviewed
        public bool AuthorExists(string name)
        {
            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Authors WHERE AuthorName = {name}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);


                    return dataTable.Rows.Count > 0;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

    }

}

