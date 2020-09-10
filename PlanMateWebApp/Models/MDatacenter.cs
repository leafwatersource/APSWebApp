using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PMSettings;
using PMPublicFunctions.PMPublicFunc;
using PMStaticModels.PlanModels;
using Microsoft.AspNetCore.Mvc;
using DataTable = System.Data.DataTable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using PMStaticModels.UserModels;

namespace PlanMateWebApp.Models
{
    /// <summary>
    /// 获取甘特图的数据
    /// </summary>
    public class GantaData
    {
        private string realOpName { get; set; }
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
        /// <summary>
        /// 获取导航栏的数据
        /// </summary>
        /// <returns></returns>
        public static string StrNavName()
        {
            List<string> groupList = new List<string>();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "SELECT distinct viewname FROM View_PmViewGroup where SYSID ='" + PMStaticModels.UserModels.PMUser.UserSysID + "' and vglobal = 'export'";
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
                if (groupList.Count == 0)
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
            for (int i = 0; i < table.Rows.Count; i++)
            {
                GantaData gantaData = new GantaData {
                    RealOpName = table.Rows[i]["realOpName"].ToString(),
                    PlannedStart = table.Rows[i]["PlannedStart"].ToString(),
                    PlannedFinish = table.Rows[i]["PlannedFinish"].ToString(),
                    ProductId = table.Rows[i]["ProductId"].ToString()
                };
                Gantt.Add(gantaData);
            }
            return Gantt;
        }
        public ActionResult<List<GantaData>> GetTodayGantattData()
        {
            List<GantaData> list = new List<GantaData>();
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "Select realOpName,PlannedStart,PlanedFinish from PMS_Bars where WorkPlanID = '" + Workplaninfo.WorkPlanId + "' and PlannedStart > '" + DateTime.Now.ToString() + "' order by realOpName,PlannedStart";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                GantaData gantaData = new GantaData {
                    RealOpName = table.Rows[i]["realOpName"].ToString(),
                    PlannedStart = table.Rows[i]["PlannedStart"].ToString(),
                    PlannedFinish = table.Rows[i]["PlannedFinish"].ToString()
                };
                list.Add(gantaData);
            }
            return list;
        }
        /// <summary>
        /// 获取订单列表的数据
        /// </summary>
        /// <returns>table</returns>
        public DataTable WorkOrderData() {
            GetWorkplanBars getWorkplanBars = new GetWorkplanBars();
            //查看要查询数据库中的哪些列
            JObject SQLWorkOrderFileds = PMAppSettings.TableFileds.SelectToken("SQLWorkOrderFiled").ToObject<JObject>();
            string SQLWorkOrderFiled = "";
            foreach (var item in SQLWorkOrderFileds)
            {
                if (string.IsNullOrEmpty(SQLWorkOrderFiled))
                {
                    SQLWorkOrderFiled += item.Key;
                }
                else
                {
                    SQLWorkOrderFiled += "," + item.Key;
                }
            }
            DataTable dt = Workplaninfo.GetWorkOrder(SQLWorkOrderFiled, "isScheduleWorkID = '1'", string.Empty);
            DataTable productId = Workplaninfo.GetWorkOrder("productID", "isScheduleWorkID = '1'", string.Empty);

            foreach (var item in SQLWorkOrderFileds)
            {
                dt.Columns[item.Key].ColumnName = item.Value.Value<string>();
            }
            DataTable AttrTable = getWorkplanBars.GetAttrTable(productId);
            JObject SQLAttrFiled = PMAppSettings.TableFileds.SelectToken("SQLAttrFiled").ToObject<JObject>();
            foreach (var item in SQLAttrFiled)
            {
                dt.Columns.Add(item.Value.Value<string>());
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int v = 0; v < AttrTable.Rows.Count; v++)
                {
                    if (AttrTable.Rows[v]["itemName"].ToString() == dt.Rows[i]["产品名称"].ToString())
                    {
                        foreach (DataColumn col in AttrTable.Columns)
                        {
                            if (col.ColumnName != "itemName")
                            {
                                dt.Rows[i][col.ColumnName] = AttrTable.Rows[v][col.ColumnName].ToString().Replace(" - ", " ").Replace("\"", "'");
                            }
                        }
                    }
                }
            }
            return dt;
        }

    }
}

    public class GetWorkplanBars
    {
    /// <summary>
    /// 查询设备下的数据信息
    /// </summary>
    /// <param name="optionPlan">设备名称</param>
    /// <param name="ViewName"></param>
    /// <returns></returns>
        public DataTable GetPlanBars(string optionPlan, string ViewName)
        {
            DataTable table = new DataTable();
            //查看要查询数据库中的哪些列
            JObject SQLWorkPlanFileds = PMAppSettings.TableFileds.SelectToken("SQLWorkPlanFiled").ToObject<JObject>();
            string SQLWorkPlanFiled = "";
            foreach (var item in SQLWorkPlanFileds)
            {
                if (string.IsNullOrEmpty(SQLWorkPlanFiled))
                {
                    SQLWorkPlanFiled += item.Key;
                }
                else
                {
                    SQLWorkPlanFiled += "," + item.Key;
                }
            }
            if (ViewName == null)
            {
                table = Workplaninfo.GetWorkPlanBars(SQLWorkPlanFiled, "OperationID = '" + optionPlan + "'", string.Empty);
            }
            else if (optionPlan == null)
            {
                //table = Workplaninfo.GetWorkPlanBars("WIPID ,itemWorkID,ProductID ,userDef_str1,allQuantity ,quantity,finishedQuantity,jobFinishedQty ,firstDemandDay,State ,route ,realOpName ,OperationID ,Lock,RealWorkTime ,PlannedStart ,PlannedFinish,PlannedSetup ,setupTime,finishStatus ,useFixStartDay ,schPri", "OperationID in ( select resName from View_pmViewGroup where VIewName = '" + ViewName + "')", string.Empty);
                table = Workplaninfo.GetWorkPlanBars(SQLWorkPlanFiled, "OperationID in ( select resName from View_pmViewGroup where VIewName = '" + ViewName + "')", string.Empty);
            }
            foreach (var item in SQLWorkPlanFileds)
            {
                table.Columns[item.Key].ColumnName = item.Value.Value<string>();
            }
            table.Columns.Add("temp", Type.GetType("System.Decimal"));
            table.Columns.Add("temp1", Type.GetType("System.Decimal"));
            for (int i = 0; i < table.Rows.Count; i++)
            {
                decimal productTime = Convert.ToDecimal(table.Rows[i][SQLWorkPlanFileds["RealWorkTime"].ToString()]) / 3600;
                productTime = Math.Round(productTime, 2);
                table.Rows[i]["temp"] = productTime;
                decimal setupTime = Convert.ToDecimal(table.Rows[i][SQLWorkPlanFileds["setupTime"].ToString()]) / 60;
                setupTime = Math.Round(setupTime, 2);
                table.Rows[i]["temp1"] = setupTime;
            }
            table.Columns.Remove(SQLWorkPlanFileds["RealWorkTime"].ToString());
            table.Columns["temp"].ColumnName = SQLWorkPlanFileds["RealWorkTime"].ToString();
            table.Columns.Remove(SQLWorkPlanFileds["setupTime"].ToString());
            table.Columns["temp1"].ColumnName = SQLWorkPlanFileds["setupTime"].ToString();
            table.AcceptChanges();
            return table;
        }

        public DataTable GetAttrTable(DataTable productID)
        {
            DataTable table = new DataTable();
            JObject SQLFileds = PMAppSettings.TableFileds.SelectToken("SQLAttrFiled").ToObject<JObject>();
            string SQLFiledStr = "itemName";
            string productStr = "";
            for (int i = 0; i < productID.Rows.Count; i++)
            {
                if (i < productID.Rows.Count - 1)
                {
                    productStr += "'" + productID.Rows[i][0].ToString() + "',";
                }
                else
                {
                    productStr += "'" + productID.Rows[i][0].ToString() + "'";

                }
            }
            foreach (var item in SQLFileds)
            {
                SQLFiledStr += "," + item.Key;
            }
            SqlCommand cmd = PMCommand.ModCmd();
            cmd.CommandText = "Select " + SQLFiledStr + " from objProduct where sysID = '" + PMStaticModels.UserModels.PMUser.UserSysID + "' and itemName in (" + productStr + ")";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            foreach (var item in SQLFileds)
            {
                table.Columns[item.Key].ColumnName = item.Value.Value<string>();
            }
            return table;
        }
    
        public DataTable GetAllPlanData()
    {
        DataTable table = new DataTable();
        JObject SQLWorkOrderFileds = PMAppSettings.TableFileds.SelectToken("SQLWorkPlanFiled").ToObject<JObject>();
        string SQLWorkPlanFiled = "";
        foreach (var item in SQLWorkOrderFileds)
        {
            if (string.IsNullOrEmpty(SQLWorkPlanFiled))
            {
                SQLWorkPlanFiled += item.Key;
            }
            else
            {
                SQLWorkPlanFiled += "," + item.Key;
            }
        }
        SqlCommand cmd = PMCommand.SchCmd();
        cmd.CommandText = "select "+ SQLWorkPlanFiled+ " from View_WorkPlansBars where WorkPlanID in (select WorkPlanID from PMS_WorkPlans where Status = 'Released' and sysid = '"+PMUser.UserSysID+"')";
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(table);
        da.Dispose();
        cmd.Connection.Dispose();
        foreach (var item in SQLWorkOrderFileds)
        {
            table.Columns[item.Key].ColumnName = item.Value.Value<string>();
        }
        return table;
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

 

