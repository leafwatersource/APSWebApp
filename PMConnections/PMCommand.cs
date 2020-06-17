using System;
using System.Data;
using System.Data.SqlClient;

namespace PMSettings
{
    public static class PMCommand
    {
        //Schedule 数据库cmd；
        public static SqlCommand SchCmd()
        {
            SqlConnection conn = new SqlConnection(PMAppSettings.Schconnstr);
            SqlCommand cmd = null;
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                    cmd = conn.CreateCommand();
                }
                catch (SqlException e)
                {
                    PMAppSettings.LiErrorMsg.Add(e.Message);
                }
            }
            return cmd;
        }
        //Modeler 数据库cmd；
        public static SqlCommand ModCmd()
        {
            SqlConnection conn = new SqlConnection(PMAppSettings.Modconnstr);
            SqlCommand cmd = new SqlCommand();
            if (conn.State ==ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                    cmd = conn.CreateCommand();
                }
                catch (SqlException e)
                {
                    PMAppSettings.LiErrorMsg.Add(e.Message);
                }
                
            }
            return cmd;
        }
        //Control 数据库cmd；
        public static SqlCommand CtrlCmd()
        {
            SqlConnection conn = new SqlConnection(PMAppSettings.Ctrlconnstr);
            SqlCommand cmd = null;
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                    cmd = conn.CreateCommand();
                }
                catch (SqlException e)
                {
                    PMAppSettings.LiErrorMsg.Add(e.Message);
                }
            }
            return cmd;
        }
    }
}
