using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using PMSettings;
using System.Text;
using System.Data;

namespace PMStaticModels.UserModels
{
    public static class PMUser
    {
        public static string EmpID { get; set; }
        public static string UserName { get; set; }
        public static string UserPass { get; set; }
        public static string UserWeb { get; set; }
        public static string UserIpAdress { get; set; }
        public static string UserGuid { get; set; }
        public static string UserSysID { get; set; }
        public static List<string> FunctionList;

        public static string GetuserGuid(string empID)
        {
            SqlCommand cmd = PMCommand.CtrlCmd();
            cmd.Parameters.Add("@EmpID", SqlDbType.VarChar).Value = empID;
            cmd.CommandText = "select userGuid from wapUserstate where empID = @EmpID";
            SqlDataReader rd = cmd.ExecuteReader();
            rd.Read();
            string userguid = rd[0].ToString();
            rd.Close();            
            cmd.Connection.Dispose();            
            return userguid;
        }
       
    }

}
