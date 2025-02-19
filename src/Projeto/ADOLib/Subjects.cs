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
    public class Subjects
    {
        private static string CnString { get; set; }

        public Subjects()
        {
            CnString = "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public Subject GetSubjectById(int subjectId)
        {
            Subject subject = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Authors WHERE AuthorId = {subjectId}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) throw new Exception("An error has occurred when trying to find the author.");

                    foreach (DataRow row in dataTable.Rows)
                    {
                        subject = new Subject()
                        {
                            SubjectId = Convert.ToInt32(row["AuthorId"]),
                            SubjectName = row["AuthorName"].ToString(),
                        };
                    }
                }

            }
            catch (Exception e)

            {
                throw new Exception(e.Message, e.InnerException);
            }

            return subject;
        }

        // Just for admins
        public int AddSubject(Subject subject)
        {
            try
            {
                // check if this verification makes sense.
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"INSERT INTO Subjects (SubjectName) VALUES ('{subject.SubjectName}')";

                    if (SubjectFinder(subject.SubjectName) is not null) throw new Exception("Author already exists");

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

        public int UpdateSubject(Subject subject)
        {
            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"UPDATE Subjects SET SubjectName = '{subject.SubjectName}' WHERE SubjectId = {subject.SubjectId}";
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

        public Subject DeleteSubjectById(int id)
        {
            Subject delSubject = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    delSubject = GetSubjectById(id);
                    string deleteQuery = $"DELETE FROM Authors WHERE AuthorId = {id}";
                    SqlTransaction transaction = connection.BeginTransaction();

                    int rowsAffected = DB.CmdExecute(connection, deleteQuery, transaction);
                    transaction.Commit();

                    return delSubject;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public static Subject SubjectFinder(string subjectName)
        {
            Subject subject = null;
            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Subjects WHERE SubjectName = {subjectName}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count == 0) return subject = null;

                    foreach(DataRow row in dataTable.Rows)
                    {
                        subject = new Subject()
                        {
                            SubjectId = Convert.ToInt32(row["SubjectId"]),
                            SubjectName = row["SubjectName"].ToString()
                        };
                    }

                    return subject;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

    }
}
