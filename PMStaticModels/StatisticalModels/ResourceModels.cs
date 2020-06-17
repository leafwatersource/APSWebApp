using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using PMSettings;

namespace PMStaticModels.StatisticalModels
{
    public static class ResourceModels
    {
        /// <summary>
        /// 获取设备组
        /// </summary>
        /// <returns>DataTable</returns>
        public static DataTable GetResGroup()
        {
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select DISTINCT ViewName from View_PmViewGroup where sysid = '" + UserModels.PMUser.UserSysID + "' and VGlobal = 'export'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable data = new DataTable();
            da.Fill(data);
            da.Dispose();
            if (data.Rows.Count < 1)
            {
                cmd.CommandText = "select DISTINCT ViewName from View_PmViewGroup  where sysid = '" + UserModels.PMUser.UserSysID + "'";
                da = new SqlDataAdapter(cmd);
                da.Fill(data);
                da.Dispose();
            }
            cmd.Connection.Close();
            return data;
        }

        /// <summary>
        /// 获取设备组下的设备名称
        /// </summary>
        /// <param name="resGroup">设备组名</param>
        /// <returns>DataTable</returns>
        public static DataTable GetResList(string resGroup)
        {
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select resname from View_PmViewGroup where sysid = '" + UserModels.PMUser.UserSysID + "' and ViewName = '" + resGroup + "'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable data = new DataTable()
            {
                TableName = resGroup
            };
            da.Fill(data);
            da.Dispose();
            cmd.Connection.Close();
            return data;
        }
        /// <summary>
        /// 获取设备稼动率的方法
        /// </summary>
        /// <param name="resName">设备名称</param>
        /// <returns>DataTable</returns>
        public static DataTable GetResUsuage(string resName)
        {
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select useDate,dayAllWorkHour,dayPlanWorkHour,dayResRatio from stsMainResUsuage where workplanid = '" + PlanModels.Workplaninfo.WorkPlanId + "' and mainResName = '" + resName + "'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable data = new DataTable()
            {
                TableName = resName
            };
            da.Fill(data);
            da.Dispose();
            cmd.Connection.Close();
            return data;
        }
        public static DataTable GetAllResUsuage()
        {
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select mainResName,useDate,dayAllWorkHour,dayPlanWorkHour,dayResRatio from stsMainResUsuage where workplanid = '" + PlanModels.Workplaninfo.WorkPlanId + "' and mainResName in (select resname from View_PmViewGroup where sysid = '33' and ViewName = 'AllBT')";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable data = new DataTable();
            da.Fill(data);
            da.Dispose();
            cmd.Connection.Close();
            return data;
        }
        public static DataTable GetUseDate()
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select distinct useDate  from stsMainResUsuage order by useDate";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Close();
            return table;
        }
    }
}
