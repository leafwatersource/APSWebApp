using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace PMStaticModels.Materials
{
    public static class MaterialsModels
    {
        public static DataTable MaterialTable(int choose)
        {
            //查找物料管理页面的数据
            DataTable data = new DataTable();
            SqlCommand cmd = PMSettings.PMCommand.SchCmd();
            if (choose == 0)
            {
                //物料数据
                cmd.CommandText = " SELECT csmItemCode,useDate, useQty, itemUnit, itemDesp, itemAttr1, buyDate, sysType FROM stsCsmItemNeed  where workPlanID = '" + PlanModels.Workplaninfo.WorkPlanId + "'";

            }
            else
            {
                //欠料表的一些数据
                cmd.CommandText = " SELECT csmItemCode,useDate, useQty, itemUnit, itemDesp, itemAttr1, buyDate FROM stsCsmItemNeed  where workPlanID = '" + PlanModels.Workplaninfo.WorkPlanId + "' and sysType = 'miss'";

            }
            //cmd.CommandText = "";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(data);
            da.Dispose();
            cmd.Connection.Dispose();           
            return data;            
        }
    }
}
