using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using PMSettings;
using PMStaticModels.PlanModels;
using System.Data.SqlClient;

namespace PMStaticModels.StatisticalModels
{
    public static class ScndUsuageModels
    {
        /// <summary>
        /// 获取辅助资源使用情况
        /// </summary>
        /// <returns></returns>
        public static DataTable GetScndUsuageData()
        {
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select scndResName,scndResType,allQty,useQty,startDateTime,endDateTime from stsScndUsuage where workplanid = '" + Workplaninfo.WorkPlanId + "' order by scndResName,startDateTime";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable data = new DataTable();
            da.Fill(data);
            da.Dispose();
            cmd.Connection.Dispose();
            return data;
        }
    }
}
