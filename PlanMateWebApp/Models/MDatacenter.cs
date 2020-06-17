using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using PMSettings;
using PMPublicFunctions.PMPublicFunc;
using PMStaticModels.PlanModels;
using Microsoft.AspNetCore.Mvc;

namespace PlanMateWebApp.Models
{
    public class GantaData
    {
        private string realOpName{get; set;}
        public string RealOpName
        {
            get
            {
                return realOpName;
            }
            set
            {
                realOpName = value;
            }
        }
        private string plannedStart { get; set; }
        public string PlannedStart
        {
            get { return plannedStart; }
            set { plannedStart = value; }
        }
        private string plannedFinish { get; set; }
        public string PlannedFinish
        {
            get { return plannedFinish; }
            set { plannedFinish = value; }
        }
        private string productId { get; set; }
        public string ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
    }
    public class MDatacenter
    {
        public int ErrorCount { get; set; }
        public int EarlyConunt { get; set; }
        public int OnTimeCount { get; set; }
        public int LateCount { get; set; }
        public int ErrorPercentage { get; set; }
        public int EarlyPercentage { get; set; }
        public int OnTimePercentage { get; set; }
        public int LatePercentage { get; set; }

        public void Datacenterorderdelay()
        {
            DataTable table = Workplaninfo.GetWorkOrder("delayDays", "isScheduleWorkID = '1'", string.Empty);            
            foreach (DataRow item in table.Rows)
            {
                if (item[0].ToString() == "")
                {

                    //异常的数量
                    ErrorCount++;
                }
                else
                {
                    if (Convert.ToInt32(item[0]) < 0)
                    {
                        EarlyConunt++;
                    }
                    else if (Convert.ToInt32(item[0]) > 0)
                    {
                        LateCount++;
                    }
                    else
                    {
                        OnTimeCount++;
                    }
                }
            }
            //计算四个值的百分比，返回给前端页面显示
            ErrorPercentage = Convert.ToInt32((Convert.ToDouble(ErrorCount) / Convert.ToDouble(table.Rows.Count)) * 100);
            EarlyPercentage = Convert.ToInt32((Convert.ToDouble(EarlyConunt) / Convert.ToDouble(table.Rows.Count)) * 100);
            OnTimePercentage = Convert.ToInt32((Convert.ToDouble(OnTimeCount) / Convert.ToDouble(table.Rows.Count)) * 100);
            LatePercentage = Convert.ToInt32((Convert.ToDouble(LateCount) / Convert.ToDouble(table.Rows.Count)) * 100);
        }
        public static string StrNavName()
        { 
            List<string> groupList = new List<string>();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "SELECT distinct viewname FROM View_PmViewGroup where SYSID ='" + PMStaticModels.UserModels.PMUser.UserSysID+ "' and vglobal = 'export'";
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                groupList.Add(rd[0].ToString());
            }            
            rd.Close();
            if (groupList.Count == 0)
            {
                cmd.CommandText = "SELECT distinct viewname FROM View_PmViewGroup where SYSID ='" + PMStaticModels.UserModels.PMUser.UserSysID + "'";
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    groupList.Add(rd[0].ToString());
                }
                rd.Close();
                if(groupList.Count == 0)
                {
                    return "没有为设备创建视图";
                }
                else
                {
                    return PMPublicFuncs.ListToJson(groupList);
                }
            }  
            else
            {
                return PMPublicFuncs.ListToJson(groupList);
            }
        }
        public static string GetPmViewGroupTable(string value)
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "SELECT resName  FROM View_PmViewGroup where sysID ='" + PMStaticModels.UserModels.PMUser.UserSysID + "' and ViewName  = '" + value + "'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            if (table.Rows.Count == 0)
            {
                return "视图中没有设备";
            }
            else
            {
                return PMPublicFuncs.DatatableToJson(table);
            }
        }
        public ActionResult<List<GantaData>> GetGanttData()
        {
            List<GantaData> Gantt = new List<GantaData>();
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select realOpName,PlannedStart,PlannedFinish,ProductID from PMS_Bars where WorkPlanID = '" + Workplaninfo.WorkPlanId + "' order by realOpName,PlannedStart";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            for (int i = 0; i <  table.Rows.Count; i++)
            {
                GantaData gantaData = new GantaData();
                gantaData.RealOpName = table.Rows[i]["realOpName"].ToString();
                gantaData.PlannedStart = table.Rows[i]["PlannedStart"].ToString();
                gantaData.PlannedFinish = table.Rows[i]["PlannedFinish"].ToString();
                gantaData.ProductId = table.Rows[i]["ProductId"].ToString();
                Gantt.Add(gantaData);
            }
            return Gantt;
        }
        public ActionResult<List<GantaData>> GetTodayGantattData()
        {
            List<GantaData> list = new List<GantaData>();
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "Select realOpName,PlannedStart,PlanedFinish from PMS_Bars where WorkPlanID = '" + Workplaninfo.WorkPlanId + "' and PlannedStart > '"+ DateTime.Now.ToString()+"' order by realOpName,PlannedStart";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                GantaData gantaData = new GantaData();
                gantaData.RealOpName = table.Rows[i]["realOpName"].ToString();
                gantaData.PlannedStart = table.Rows[i]["PlannedStart"].ToString();
                gantaData.PlannedFinish = table.Rows[i]["PlannedFinish"].ToString();
                list.Add(gantaData);
            }
            return list;
        }
    }

    public class GetWorkplanBars
    {
        public string GetPlanBars(string optionPlan, string ViewName)
        {
            DataTable table = new DataTable();
          
            if (ViewName == null)
            {
                table = Workplaninfo.GetWorkPlanBars("WIPID ,itemWorkID,ProductID ,userDef_str1,allQuantity ,quantity,finishedQuantity,jobFinishedQty ,firstDemandDay,State ,route ,realOpName ,OperationID ,Lock,RealWorkTime ,PlannedStart ,PlannedFinish,PlannedSetup ,setupTime,finishStatus ,useFixStartDay ,schPri", "OperationID = '" + optionPlan + "'", string.Empty);
            }
            else if (optionPlan == null)
            {
                table = Workplaninfo.GetWorkPlanBars("WIPID ,itemWorkID,ProductID ,userDef_str1,allQuantity ,quantity,finishedQuantity,jobFinishedQty ,firstDemandDay,State ,route ,realOpName ,OperationID ,Lock,RealWorkTime ,PlannedStart ,PlannedFinish,PlannedSetup ,setupTime,finishStatus ,useFixStartDay ,schPri", "OperationID in ( select resName from View_pmViewGroup where VIewName = '" + ViewName + "')", string.Empty);
            }

            table.Columns[0].ColumnName = "主工单";
            table.Columns[1].ColumnName = "工单";
            table.Columns[2].ColumnName = "产品";
            table.Columns[3].ColumnName = "描述";
            table.Columns[4].ColumnName = "工单总数";
            table.Columns[5].ColumnName = "排程数量";
            table.Columns[6].ColumnName = "完成数量";
            table.Columns[7].ColumnName = "总完成数";
            table.Columns[8].ColumnName = "需求日期";
            table.Columns[9].ColumnName = "工单状态";
            table.Columns[10].ColumnName = "流程";
            table.Columns[11].ColumnName = "工序";
            table.Columns[12].ColumnName = "设备";
            table.Columns[13].ColumnName = "锁定";
            table.Columns[14].ColumnName = "生产时长";
            table.Columns[15].ColumnName = "计划开始";
            table.Columns[16].ColumnName = "计划结束";
            table.Columns[17].ColumnName = "切换开始";
            table.Columns[18].ColumnName = "切换时间";
            table.Columns[19].ColumnName = "完成状态";
            table.Columns[20].ColumnName = "固定开工"; 
            table.Columns[21].ColumnName = "优先级";
            table.Columns.Add("temp", Type.GetType("System.Decimal"));
            table.Columns.Add("temp1", Type.GetType("System.Decimal"));
            for (int i = 0; i < table.Rows.Count; i++)
            {
                decimal productTime = Convert.ToDecimal(table.Rows[i][14]) / 3600;
                productTime = Math.Round(productTime, 2);
                table.Rows[i][22] = productTime;
                decimal setupTime = Convert.ToDecimal(table.Rows[i][18]) / 60;
                setupTime = Math.Round(setupTime, 2);
                table.Rows[i][23] = setupTime;

            }
            table.Columns.RemoveAt(14);
            table.Columns[21].SetOrdinal(14);
            table.Columns[14].ColumnName = "生产时长";
            table.Columns.RemoveAt(18);
            table.Columns[21].SetOrdinal(18);
            table.Columns[18].ColumnName = "切换时间";
            table.AcceptChanges();
            return PMPublicFuncs.DatatableToJson(table);
        }
    }

    public class OrderForm
    {
        public static string[] ErrorTime;
        public static string[] EarlyTime;
        public static string[] OnTime;
        public static string[] LateTime;
        public static string[] CountTime;
        public static int[] Allordercount;
        public List<DateTime> LiDemandDay()
        {
            List<DateTime> list = new List<DateTime>();
            foreach (DataRow dr in Workplaninfo.GetWorkOrder("distinct firstDemandDay", "isScheduleWorkID = 1", "firstDemandDay").Rows)
            {
                list.Add(Convert.ToDateTime(dr[0]));
            }
            return list;
        }
        public void Pieorder()
        {
            List<DateTime> list = LiDemandDay();
            DataTable table = Workplaninfo.GetWorkOrder("workID,productID,allQuantity,desp,planStartTime,planFinishTime,firstDemandDay,delayDays", "isScheduleWorkID = 1", string.Empty);
            ErrorTime = new string[list.Count];
            EarlyTime = new string[list.Count];
            OnTime = new string[list.Count];
            LateTime = new string[list.Count];
            CountTime = new string[list.Count];
            Allordercount = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                DataRow[] row = table.Select("firstDemandDay='" + list[i] + "'");
                int lateCount = 0;
                int earlyCount = 0;
                int ontimeCount = 0;
                int errorCount = 0;
                foreach (var item in row)
                {
                    if (item[7].ToString() == "")
                    {
                        errorCount++;
                    }
                    else
                    {
                        if (Convert.ToInt32(item[7]) > 0)
                        {
                            lateCount++;
                            //LateTime[i]++;
                        }
                        else if (Convert.ToInt32(item[7]) < 0)
                        {
                            earlyCount++;
                        }
                        else if (Convert.ToInt32(item[7]) == 0)
                        {
                            ontimeCount++;
                        }
                    }
                }
                Allordercount[i] = row.Count();
                LateTime[i] = (Convert.ToDecimal(lateCount) / Convert.ToDecimal(row.Count()) * 100).ToString("0.00");
                EarlyTime[i] = (Convert.ToDecimal(earlyCount) / Convert.ToDecimal(row.Count()) * 100).ToString("0.00");
                OnTime[i] = (Convert.ToDecimal(ontimeCount) / Convert.ToDecimal(row.Count()) * 100).ToString("0.00");
                ErrorTime[i] = (Convert.ToDecimal(errorCount) / Convert.ToDecimal(row.Count()) * 100).ToString("0.00");
            }
        }
    }
}

