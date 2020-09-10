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
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace PlanMateWebApp.Controllers
{
   

    public class DatacenterController : Controller
    {
       
        public IActionResult Index()
        {
            string[] UserIDList = PMUser.UserMessage.Select(x => x.EmpID).ToArray();
            bool UserID = UserIDList.Contains<string>(HttpContext.Request.Cookies["EmpID"]);
            //if (PMUser.EmpID ==string.Empty)
            //{
            //    return RedirectToAction("Index", "Index");
            //}
            if (!UserID)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(HttpContext.Request.Cookies["EmpID"]))
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
        public IActionResult Implementation()
        {
            string[] UserIDList = PMUser.UserMessage.Select(x => x.EmpID).ToArray();
            bool UserID = UserIDList.Contains<string>(HttpContext.Request.Cookies["EmpID"]);
            if (!UserID)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(HttpContext.Request.Cookies["EmpID"]))
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

            string[] UserIDList = PMUser.UserMessage.Select(x => x.EmpID).ToArray();
            bool UserID = UserIDList.Contains<string>(HttpContext.Request.Cookies["EmpID"]);
            //if (PMUser.EmpID ==string.Empty)
            //{
            //    return RedirectToAction("Index", "Index");
            //}
            if (!UserID)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(HttpContext.Request.Cookies["EmpID"]))
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

            string[] UserIDList = PMUser.UserMessage.Select(x => x.EmpID).ToArray();
            bool UserID = UserIDList.Contains<string>(HttpContext.Request.Cookies["EmpID"]);
            //if (PMUser.EmpID ==string.Empty)
            //{
            //    return RedirectToAction("Index", "Index");
            //}
            if (!UserID)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(HttpContext.Request.Cookies["EmpID"]))
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

            string[] UserIDList = PMUser.UserMessage.Select(x => x.EmpID).ToArray();
            bool UserID = UserIDList.Contains<string>(HttpContext.Request.Cookies["EmpID"]);
            //if (PMUser.EmpID ==string.Empty)
            //{
            //    return RedirectToAction("Index", "Index");
            //}
            if (!UserID)
            {
                return RedirectToAction("Index", "Index");
            }
            else if (HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(HttpContext.Request.Cookies["EmpID"]))
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
            if (tableName == "WorkOrder")
            {
                JObject order = PMAppSettings.TableFileds.SelectToken("SQLWorkOrderFiled").ToObject<JObject>();
                foreach (var item in PMAppSettings.TableFileds.SelectToken("SQLAttrFiled").ToObject<JObject>())
                {
                    order.Add(item.Key, item.Value);
                }
                return Ok(order);
            }
            else if (tableName == "WorkPlan")
            {
                return Ok(PMAppSettings.TableFileds.SelectToken("SQLWorkPlanFiled").ToObject<JObject>());
            }
            else if (tableName=="History")
            {
                return Ok(PMAppSettings.TableFileds.SelectToken("HistoryTableFiled").ToObject<JObject>());
            }
            return Ok("表格的列查询错误");
        }
        public ActionResult DTworkOrder() {
            MDatacenter mDatacenter = new MDatacenter();
            DataTable dt = mDatacenter.WorkOrderData();
            JObject tableData = new JObject {
                { "total", dt.Rows.Count },
                { "rows", JsonConvert.SerializeObject(dt) }
            };
            JObject data = new JObject {
                { "code", "0"},
                { "data", tableData},
                { "msg", "successful"}
            };
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
            DataTable table =  PlanInfo.GetPlanBars(plan,ViewName);
            return PMPublicFuncs.DatatableToJson(table);
        }
        /// <summary>
        /// 执行计划的表格
        /// </summary>
        /// <param name="plan">计划名称</param>
        /// <param name="ViewName">设备名称</param>
        /// <returns></returns>
        public ActionResult WorkPlanBar(string plan, string ViewName) {
            GetWorkplanBars PlanInfo = new GetWorkplanBars();
            DataTable table =  PlanInfo.GetPlanBars(plan, ViewName);
            string JsonString =  JsonConvert.SerializeObject(table);
            JObject tableData = new JObject {
                { "total", table.Rows.Count},
                { "rows", JsonString}
            };
            JObject data = new JObject {
                { "code", "0"},
                {"data", tableData },
                { "msg", "successful"}
            };
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
        public ActionResult GetHistoryTable()
        {
            MDHistory mDHistory = new MDHistory();

            return Ok(mDHistory.MGetHistoryData());
        }

        /// <summary>
        /// 订单列表的导出功能
        /// </summary>
        /// <returns>路径让浏览器下载</returns>
        public ActionResult ExportDataCenter()
        {
            MDatacenter mDatacenter = new MDatacenter();
            DataTable table = mDatacenter.WorkOrderData();
            string path = ExportExcel.Excel(table, "订单列表_"+PMUser.UserGuid+".xlsx", "订单数据");
            Task.Run(() =>
            {
                Thread.Sleep(30000);
                ExportExcel.DelExcel("订单列表_" + PMUser.UserGuid);
            });
            return Ok(path);
        }
   

        /// <summary>
        /// 导出计划列表
        /// </summary>
        /// <returns>路径让浏览器下载</returns>
        public ActionResult ExportPlanData()
        {
            GetWorkplanBars getWorkplanBars = new GetWorkplanBars();
            DataTable table = getWorkplanBars.GetAllPlanData();
            string path = ExportExcel.Excel(table, "计划列表_" + PMUser.UserGuid + ".xlsx", "计划数据");
            Task.Run(() =>
            {
                Thread.Sleep(30000);
                ExportExcel.DelExcel("计划列表_" + PMUser.UserGuid);
            });
            return Ok(path);
        }
    }
}