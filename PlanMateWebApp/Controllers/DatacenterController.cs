using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlanMateWebApp.Models;
using PMPublicFunctions.PMPublicFunc;
using PMStaticModels.PlanModels;
using PMStaticModels.UserModels;
using PMSettings;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PlanMateWebApp.Controllers
{
    public class DatacenterController : Controller
    {
        public IActionResult Index()
        {            
            if (PMUser.EmpID ==string.Empty)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(PMUser.EmpID))
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                //获取计划信息
                Workplaninfo.GetWorkPlanInfo(string.Empty);
                ViewBag.Owner = Workplaninfo.Owner;
                ViewBag.WorkPlanId = Workplaninfo.WorkPlanId;
                ViewBag.WorkPlanName = Workplaninfo.WorkPlanName;
                ViewBag.ReleaseTime = Workplaninfo.ReleaseTime;
                //获取order统计信息
                MDatacenter data = new MDatacenter();
                data.Datacenterorderdelay();
                ViewBag.ErrorPercentage = data.ErrorPercentage;
                ViewBag.LatePercentage = data.LatePercentage;
                ViewBag.OnTimePercentage = data.OnTimePercentage;
                ViewBag.EarlyPercentage = data.EarlyPercentage;
                OrderForm order = new OrderForm();
                order.Pieorder();
                return View(data);
            }
        }
        public IActionResult TextPage()
        {
            return View();
        }
        public IActionResult Implementation()
        {           
            if (PMUser.EmpID == string.Empty)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(PMUser.EmpID))
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                Workplaninfo.GetWorkPlanInfo(string.Empty);
                ViewBag.Owner = Workplaninfo.Owner;
                ViewBag.WorkPlanId = Workplaninfo.WorkPlanId;
                ViewBag.WorkPlanName = Workplaninfo.WorkPlanName;
                ViewBag.ReleaseTime = Workplaninfo.ReleaseTime;
                return View();
            }
        }
        public IActionResult HistoryData()
        {            
            if (PMUser.EmpID == string.Empty)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(PMUser.EmpID))
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                MDatacenter data = new MDatacenter();
                data.Datacenterorderdelay();
                Workplaninfo.GetWorkPlanInfo(string.Empty);
                return View();
            }
          
        }
        public IActionResult Record()
        {
            if (PMUser.EmpID == String.Empty)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(PMUser.EmpID))
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                return View();
            }
        }
        public IActionResult UserSettings()
        {
            if (PMUser.EmpID == String.Empty)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(PMUser.EmpID))
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                return View();
            }
        }
        /// <summary>
        /// WorkOrderFild:返回数据中心工单页面的所展示的列
        /// </summary>
        /// <returns></returns>
        public ActionResult TableFiled(string tableName) {
            string jsonfile ="filed.json";//JSON文件路径
            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject filed = (JObject)JToken.ReadFrom(reader);
                    if (tableName == "WorkOrder")
                    {
                        return Ok(filed["ShowWorkOrderFiled"]);

                    }
                    else if(tableName == "WorkPlan")
                    {
                        return Ok(filed["ShowWorkPlanFiled"]);
                    }
                    else
                    {
                        return Ok(111);
                    }
                }
            }
        }
        public ActionResult DTworkOrder() {
            DataTable dt = Workplaninfo.GetWorkOrder("workID,productID,allQuantity,desp,planStartTime,planFinishTime,firstDemandDay,delayDays", "isScheduleWorkID = '1'", string.Empty);
            DataTable productId = Workplaninfo.GetWorkOrder("productID", "isScheduleWorkID = '1'", string.Empty);
            dt.Columns["productID"].ColumnName = "产品名称";
            DataTable AttrTable = Workplaninfo.GetAttrTable(productId);
            dt.Columns["workID"].ColumnName = "工单号码";
            dt.Columns["allQuantity"].ColumnName = "工单总数";
            dt.Columns["desp"].ColumnName = "描述";
            dt.Columns["planStartTime"].ColumnName = "计划开始时间";
            dt.Columns["planFinishTime"].ColumnName = "计划结束时间";
            dt.Columns["firstDemandDay"].ColumnName = "需求日期";
            dt.Columns["delayDays"].ColumnName = "延迟天数";
            for (int i = 1; i <= Convert.ToInt32(PMAppSettings.ItemAttrCount); i++)
            {
                if (i == Convert.ToInt32(PMAppSettings.ItemAttrCount))
                {
                    AttrTable.Columns["itemWeight"].ColumnName = PMAppSettings.ItemGroup.Rows[0]["itemWeight"].ToString();
                    dt.Columns.Add(PMAppSettings.ItemGroup.Rows[0]["ItemWeight"].ToString());
                }
                else
                {
                    AttrTable.Columns["ItemAttr" + i].ColumnName = PMAppSettings.ItemGroup.Rows[0]["ItemAttr" + i].ToString();
                    dt.Columns.Add(PMAppSettings.ItemGroup.Rows[0]["ItemAttr" + i].ToString());
                }
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
            string JsonString = string.Empty;
            JsonString = JsonConvert.SerializeObject(dt);
            JObject tableData = new JObject();
            tableData.Add("total", dt.Rows.Count);
            tableData.Add("rows", JsonString);
            JObject data = new JObject();
            data.Add("code", "0");
            data.Add("data", tableData);
            data.Add("msg", "successful");
            return Ok(data);
        }
        public string NavName()
        {
            //此方法是加载左边导航条的标签
            return  MDatacenter.StrNavName();
        }
        public string GetNavTable(string value)
        {
            //此方法是获取表格中的数据
            return MDatacenter.GetPmViewGroupTable(value);
        }
        public string GetWorkPlanBars(string plan, string ViewName)
        {
            GetWorkplanBars PlanInfo = new GetWorkplanBars();
            DataTable table = new DataTable();
            table = PlanInfo.GetPlanBars(plan,ViewName);
            return PMPublicFuncs.DatatableToJson(table);
        }
        public ActionResult WorkPlanBar(string plan, string ViewName) {
            GetWorkplanBars PlanInfo = new GetWorkplanBars();
            DataTable table = new DataTable();
            table = PlanInfo.GetPlanBars(plan, ViewName);
            string JsonString = string.Empty;
            JsonString = JsonConvert.SerializeObject(table);
            JObject tableData = new JObject();
            tableData.Add("total", table.Rows.Count);
            tableData.Add("rows", JsonString);
            JObject data = new JObject();
            data.Add("code", "0");
            data.Add("data", tableData);
            data.Add("msg", "successful");
            return Ok(data);

        }
        public string StatisticalData()
        {
            //此方法是初始化图表中的x轴数据返回的是数组类型的数据
            OrderForm order = new OrderForm();
            return PMPublicFuncs.ListToJson(order.LiDemandDay());
        }
        public string OrderPieCount()
        {
            //此方法是初始化图表的样式中的y轴的数据数据返回的是字符5串类型的json数据
            string early = "{" + "\"" + "EarlyTime" + "\"" + ":[";
            string error = "{" + "\"" + "ErrorTime" + "\"" + ":[";
            string late = "{" + "\"" + "LateTime" + "\"" + ":[";
            string time = "{" + "\"" + "OnTime" + "\"" + ":[";
            string allorder = "{" + "\"" + "Allorder" + "\"" + ":[";
            for (int i = 0; i < OrderForm.OnTime.Length; i++)
            {
                if (i < OrderForm.OnTime.Length - 1)
                {
                    early += OrderForm.EarlyTime[i].ToString() + ",";
                    error += OrderForm.ErrorTime[i].ToString() + ",";
                    late += OrderForm.LateTime[i].ToString() + ",";
                    time += OrderForm.OnTime[i].ToString() + ",";
                    allorder += OrderForm.Allordercount[i].ToString() + ",";
                }
                else
                {
                    early += OrderForm.EarlyTime[i].ToString() + "]";
                    error += OrderForm.ErrorTime[i].ToString() + "]";
                    late += OrderForm.LateTime[i].ToString() + "]";
                    time += OrderForm.OnTime[i].ToString() + "]";
                    allorder += OrderForm.Allordercount[i].ToString() + "]";
                }
            }
            string arr = "[" + early + "}," + error + "}," + late + "}," + time + "}," + allorder + "}" + "]";
            return arr;
        }
        public ActionResult<List<GantaData>> GetGantt() {
            MDatacenter mDatacenter = new MDatacenter();
            return mDatacenter.GetGanttData();
        }
        public ActionResult<List<GantaData>> GetToDayGantt()
        {
            MDatacenter mDatacenter = new MDatacenter();
            return mDatacenter.GetTodayGantattData();
        }
    }
}