using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using PMSettings;

namespace PMStaticModels.StatisticalModels
{
    public static class StatisticalModels
    {
        public static string OpName { get; set; }
        public static DataTable GetProductOutPut(int isfinal,string OpName)
        {
            SqlCommand cmd = PMCommand.SchCmd();
            string itemAttrstr = "\'\'";
            for (int i = 0; i <Convert.ToInt32( PMSettings.PMAppSettings.ItemAttrCount); i++)
            {
                if (itemAttrstr != "\'\'")
                {
                    itemAttrstr += ",itemattr" + (i + 1);
                }
                else
                {
                    itemAttrstr = "";
                    itemAttrstr += "itemattr" + (i + 1);
                }
            }
            if (isfinal == 0 && !string.IsNullOrEmpty(OpName))
            {
                cmd.CommandText = "select itemname,opName,outputqty,exportdate," + itemAttrstr + ",desp from stsItemOpDailyExport where workplanid = '" + PlanModels.Workplaninfo.WorkPlanId + "' and isFinial ='0' and opname='"+OpName+"'";
            }
            else if (isfinal == 0 && string.IsNullOrEmpty(OpName))
            {
                cmd.CommandText = "select itemname,opName,outputqty,exportdate," + itemAttrstr + ",desp from stsItemOpDailyExport where workplanid = '" + PlanModels.Workplaninfo.WorkPlanId + "' and isFinial ='0'";
            }
            else if (isfinal == 1)
            {
                
                cmd.CommandText = "select itemname,outputqty,exportdate," + itemAttrstr + ",desp from stsItemOpDailyExport where workplanid = '" + PlanModels.Workplaninfo.WorkPlanId + "' and isFinial ='1'";
            }
            else
            {
                cmd.CommandText = "select itemname,outputqty,exportdate," + itemAttrstr + ",desp from stsItemOpDailyExport where workplanid = '" + PlanModels.Workplaninfo.WorkPlanId + "'";
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable data = new DataTable();
            da.Fill(data);
            da.Dispose();
            cmd.Connection.Dispose();
            return data;
        }
    }
}
