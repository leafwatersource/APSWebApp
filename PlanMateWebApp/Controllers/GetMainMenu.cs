 using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMSettings;
namespace PlanMateWebApp.Controllers
{
    public class GetMainMenu
    {
        public ActionResult<string> GetMenu()
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select * from STS_MainMenu";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            return "";
        }
    }
}
