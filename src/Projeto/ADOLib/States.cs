using System.Data;
using LibDB;
using Microsoft.Data.SqlClient;
using static ADOLib.Model.Model;

namespace ADOLib
{
    public class States
    {
        private string CnString { get; set; }

        public States()
        {
            CnString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        public State GetStateById(int stateId)
        {
            State state = null;
            
            try
            {
                using(SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM States WHERE StateId = {stateId}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count == 0) return state;

                    foreach(DataRow row in dataTable.Rows)
                    {
                        state = new State()
                        {
                            StateId = Convert.ToInt32(row["StateId"]),
                            StateName = row["StateName"].ToString(),
                        };
                    }

                    return state;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public State GetStateByName(string stateName)
        {
            State state = null;

            try
            {
                using (SqlConnection connection = DB.Open(CnString))
                {
                    string query = $"SELECT * FROM States WHERE StateName = {stateName}";
                    DataTable dataTable = DB.GetSQLRead(connection, query);

                    if (dataTable.Rows.Count == 0) return state;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        state = new State()
                        {
                            StateId = Convert.ToInt32(row["StateId"]),
                            StateName = row["StateName"].ToString(),
                        };
                    }

                    return state;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

    }
}
