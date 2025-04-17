using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Configuration;

namespace My_Restaurant.Models.Repo
{
    public class SQLHelper : ISQLHelper
    {
        public static string GstrConnMVC_TestDB = ConfigurationManager.ConnectionStrings["MVC_TestDB"].ConnectionString;

        public SqlConnection GetSqlConnection()
        {
            SqlConnection sqlConnection = new SqlConnection(GstrConnMVC_TestDB);
            return sqlConnection;
        }

        public SqlDataReader GetDataReader(string strSQL, SqlParameter[] parameters , SqlConnection sqlConnection)
        {
            sqlConnection.Open();
            SqlCommand selectCommand = new SqlCommand(strSQL, sqlConnection);
            if (parameters != null)
            {
                selectCommand.Parameters.AddRange(parameters);
            }
            SqlDataReader selectDataReader = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
            return selectDataReader;
        }

        public int GetInt(string strSelectSQL , SqlParameter[] parameters)
        {
            int intSelect;
            using (SqlConnection connMVC_TestDB = new SqlConnection(GstrConnMVC_TestDB))
            {
                connMVC_TestDB.Open();
                SqlCommand selectCommand = new SqlCommand(strSelectSQL, connMVC_TestDB);
                if (parameters != null)
                {
                    selectCommand.Parameters.AddRange(parameters);
                }
                SqlDataReader selectDataReader = selectCommand.ExecuteReader();
                selectDataReader.Read();
                intSelect = selectDataReader.GetInt32(0);
                return intSelect;
            }
        }

        public void CUD(string strSQL, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = new SqlConnection(GstrConnMVC_TestDB))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection);
                if (parameters != null)
                {
                    sqlCommand.Parameters.AddRange(parameters);
                }
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}