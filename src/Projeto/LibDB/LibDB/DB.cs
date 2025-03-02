using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LibDB
{
    public class DB
    {
        public static SqlConnection Open(string ConnectionString)
        {
            SqlConnection db = default(SqlConnection);
            db = new SqlConnection();
            db.ConnectionString = ConnectionString;
            db.Open();

            return db;
        }

        public static void Close(SqlConnection cn)
        {
            try { cn.Close(); } catch { }
            try { cn.Dispose(); } catch { }
        }

        public static void Close(DataSet ds)
        {
            try { ds.Dispose(); } catch { }
        }

        public static void Close(DataTable dt)
        {
            try { dt.Dispose(); } catch { }
        }

        public static void Close(SqlTransaction pTrans)
        {
            try { pTrans.Dispose(); } catch { }
        }

        public static void Close(SqlDataAdapter da)
        {
            try { da.Dispose(); } catch { }
        }

        public static DataTable GetSQLRead(SqlConnection cn, string sSQL, SqlTransaction pTransaction = null)
        {
            SqlDataAdapter daDB = default(SqlDataAdapter);
            DataTable dtt = new DataTable();

            daDB = GetAdapterRead(cn, sSQL, pTransaction);
            daDB.Fill(dtt);

            return dtt;
        }

        public static DataTable GetSQLWrite(SqlConnection cn, string sSQL, ref SqlDataAdapter daDB, SqlTransaction pTransaction = null)
        {
            DataTable dt = new DataTable();

            daDB = GetAdapterWrite(cn, sSQL, pTransaction);
            daDB.Fill(dt);

            return dt;
        }

        public static List<Dictionary<string, Object>> ToDictionary(DataTable dt)
        {
            List<Dictionary<string, Object>> lst = new List<Dictionary<string, Object>>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, Object> lstRecord = new Dictionary<string, Object>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    lstRecord.Add(dt.Columns[i].ColumnName, dr[i]);
                }

                lst.Add(lstRecord);
            }

            return lst;
        }

        public static List<List<Object>> ToList(DataTable dt)
        {
            List<List<Object>> lst = new List<List<Object>>();

            foreach (DataRow dr in dt.Rows)
            {
                List<Object> lstRecord = new List<Object>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    lstRecord.Add(dr[i]);
                }

                lst.Add(lstRecord);
            }

            return lst;
        }

        public static int CmdExecute(SqlConnection cn, string sQry, SqlTransaction pTransaction = null)
        {
            SqlCommand command = new SqlCommand(sQry, cn, pTransaction);
            return command.ExecuteNonQuery();
        }

        public static int CmdExecute(string sServer, string sBDName, string sQry, SqlTransaction pTransaction = null)
        {
            int rc = 0;

            SqlConnection cn = null;
            try
            {
                string connectionString =
                    "Data Source=" + sServer + ";Initial Catalog=" + sBDName + ";"
                    + "Integrated Security=true";


                cn = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sQry, cn, pTransaction);
                cn.Open();
                rc = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                Close(cn);
            }

            return rc;
        }

        public static SqlDataAdapter GetAdapterRead(SqlConnection pConnection, string stSQL, SqlTransaction pTransaction = null, int TimeoutSeconds = 300)
        {
            SqlDataAdapter oDA = default(SqlDataAdapter);
            oDA = new SqlDataAdapter(stSQL, pConnection);
            oDA.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            oDA.SelectCommand.Transaction = pTransaction;
            oDA.SelectCommand.CommandTimeout = TimeoutSeconds;
            return oDA;
        }

        public static SqlDataAdapter GetAdapterWrite(SqlConnection pConnection, string stSQL, SqlTransaction pTransaction = null)
        {
            SqlDataAdapter oDA = default(SqlDataAdapter);
            SqlCommandBuilder oCB = default(SqlCommandBuilder);
            SqlCommand cmd = default(SqlCommand);
            cmd = new SqlCommand(stSQL, pConnection, pTransaction);
            oDA = new SqlDataAdapter(cmd);
            oDA.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            oCB = new SqlCommandBuilder(oDA);
            oCB.QuotePrefix = "[";
            oCB.QuoteSuffix = "]";
            return oDA;
        }
    }
}
