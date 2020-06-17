using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using PMSettings;
using PMStaticModels;

namespace PMStaticModels.PlanModels
{
   
    public static class Workplaninfo
    {
        public static string Owner { get; set; }
        public static string WorkPlanId { get; set; }
        public static string WorkPlanName { get; set; }
        public static string ReleaseTime { get; set; }

        public static void GetWorkPlanInfo(string colName)
        {
            SqlCommand cmd = PMCommand.SchCmd();
            if(string.IsNullOrEmpty(colName))
            {
                cmd.CommandText = "select * from PMS_WorkPlans where sysID = '" + UserModels.PMUser.UserSysID + "' and Status = '" + PMAppSettings.PMPlState + "'";
            }
            else
            {
                cmd.CommandText = "select " + colName + " from PMS_WorkPlans where sysID = '" + UserModels.PMUser.UserSysID + "' and Status = '" + PMAppSettings.PMPlState + "'";
            }
            //cmd.CommandText = "select Owner,WorkPlanId,WorkPlanName from PMS_WorkPlans where sysID = '" + AppSettings.PMSysid + "' and Status = '" + AppSettings.PMPlState + "'";
            SqlDataReader rd = cmd.ExecuteReader();
            rd.Read();
            Owner = rd["Owner"].ToString();
            WorkPlanName = rd["WorkPlanName"].ToString();
            WorkPlanId = rd["WorkPlanId"].ToString();
            DateTime tmp = DateTime.Now;
            try
            {
                Convert.ToDateTime(rd["planReleaseTime"].ToString());
            }
            catch (Exception)
            {

            }
            
            ReleaseTime = tmp.Year.ToString() + "/" + tmp.Month.ToString() + "/" + tmp.Day.ToString() + " " + tmp.Hour.ToString() + ":" + tmp.Minute.ToString();
            rd.Close();
            cmd.Connection.Dispose();
        }

        public static DataTable GetWorkOrder(string colName,string filter,string ordertype)
        {
            // colName: colname1,colname2,
            // filter:colname1 = 'value1',and colname2 = 'value2'
            // ordertype colname1,colname2 DESC
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            string cmdselectstring;
            if (string.IsNullOrEmpty(colName))
            {
                cmdselectstring = "SELECT * FROM User_WorkOrder";
            }
            else
            {
                cmdselectstring = "SELECT " + colName + " FROM User_WorkOrder";
            }
            string cmdfilterstring;
            if (string.IsNullOrEmpty(filter))
            {
                cmdfilterstring = " where workPlanID in (SELECT workPlanID FROM PMS_WorkPlans where sysID = '" + UserModels.PMUser.UserSysID + "' and Status = '" + PMAppSettings.PMOcState + "')";
            }
            else
            {
                cmdfilterstring = " where " + filter + " and workPlanID in (SELECT workPlanID FROM PMS_WorkPlans where sysID = '" + UserModels.PMUser.UserSysID + "' and Status = '" + PMAppSettings.PMOcState + "')";
            }
            cmd.CommandText = cmdselectstring + cmdfilterstring;
            if(string.IsNullOrEmpty(ordertype) == false)
            {
                cmd.CommandText += "order by " + ordertype;
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();            
            return table;
        }
        public static DataTable GetAttrTable(DataTable table)
        {
            //此方法是获取五个属性值，返回的是要给table，传入的table是产品的id值
            string temp = string.Empty;
            DataTable NewTable = new DataTable();
            string attrstr = string.Empty;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (i < table.Rows.Count - 1)
                {
                    temp += "'" + table.Rows[i][0].ToString() + "',";
                }
                else
                {
                    temp += "'" + table.Rows[i][0].ToString() + "'";

                }
            }
            for (int i = 1; i <=Convert.ToInt32( PMAppSettings.ItemAttrCount); i++)
            {
                if (i == 11)
                {
                    attrstr += ",itemWeight";
                    continue;
                }
                if (attrstr == string.Empty)
                {
                    attrstr = "itemName,itemAttr" + i;
                }
                else
                {
                    attrstr += ",itemAttr" + i;
                }
            }
            SqlCommand cmd = PMCommand.ModCmd();
            cmd.CommandText = "Select "+ attrstr + " from objProduct where sysID = '"+ UserModels.PMUser.UserSysID + "' and itemName in (" + temp + ")";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(NewTable);
            da.Dispose();
            cmd.Connection.Dispose();
            return NewTable;
        }
        public static DataTable GetWorkPlanBars(string colName,string filter,string ordertype)
        {
            // colName: colname1,colname2,
            // filter:colname1 = 'value1',and colname2 = 'value2'
            // ordertype colname1,colname2 DESC
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            string cmdselectstring;
            if (string.IsNullOrEmpty(colName))
            {
                cmdselectstring = "SELECT * FROM View_WorkPlansBars";
            }
            else
            {
                cmdselectstring = "SELECT " + colName + " FROM View_WorkPlansBars";
            }
            string cmdfilterstring;
            if (string.IsNullOrEmpty(filter))
            {
                cmdfilterstring = " where WorkPlanID in (select WorkPlanID from PMS_WorkPlans where Status = '" + PMAppSettings.PMPlState + "' and sysid = '" + UserModels.PMUser.UserSysID + "')";
            }
            else
            {
                cmdfilterstring = " where " + filter + " and WorkPlanID in (select WorkPlanID from PMS_WorkPlans where Status = '" + PMAppSettings.PMPlState + "' and sysid = '" + UserModels.PMUser.UserSysID + "')";
            }
            cmd.CommandText = cmdselectstring + cmdfilterstring;
            if (string.IsNullOrEmpty(ordertype) == false)
            {
                cmd.CommandText += "order by " + ordertype;
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            return table;
        }
    }
}
