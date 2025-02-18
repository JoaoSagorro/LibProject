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
    public class Subjects
    {
        private static string CnString { get; set; }

        public Subjects()
        {
            CnString = "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public static int SubjectFinder(string subject)
        {
            int subjectId = 0;
            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM Subjects WHERE SubjectName = {subject}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count != 1) throw new Exception("An error has occurred.");

                    foreach(DataRow row in dataTable.Rows)
                    {
                        subjectId = Convert.ToInt32(row["SubjectId"]);
                    }

                    return subjectId;
                }
            }
            catch(Exception e)
            {
                return subjectId = -1;
                throw new Exception(e.Message, e.InnerException);
            }
        }

    }
}
