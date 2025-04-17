using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace My_Restaurant.Models.Repo
{
    interface ISQLHelper
    {
        SqlConnection GetSqlConnection();

        SqlDataReader GetDataReader(string strSQL, SqlParameter[] parameters , SqlConnection sqlConnection);

        int GetInt(string strSelectSQL , SqlParameter[] parameters);

        void CUD(string strSQL, SqlParameter[] parameters);

    }
}
