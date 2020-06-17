using PMSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PlanMateWebApp.Models
{
    public class Loglist
    {
        public DataTable Getlist()
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.CtrlCmd();
            cmd.CommandText = "select * from wapUserlog";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            return table;
        }

    }
}
