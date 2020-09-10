using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanMateWebApp.Models;
using PMStaticModels.UserModels;
namespace PlanMateWebApp.Controllers
{
    public class PhoneController : Controller
    {
        public IActionResult Index()
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
                return View();
            }
        }
        public IActionResult Text()
        {
            if (string.IsNullOrEmpty( PMUser.EmpID))
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
        public IActionResult Test()
        {
            if (string.IsNullOrEmpty(PMUser.EmpID))
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
        public IActionResult Secondly()
        {
            if (string.IsNullOrEmpty(PMUser.EmpID))
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
        public ActionResult<DataTable> Get_menu()
        {
            //获取菜单按钮
            MDphone mDphone = new MDphone();
            return mDphone.GetMenu();
        }
        public ActionResult<DataTable> Get_plan(string resname)
        {
            //获取所选设备的所有工单
            MDphone mDphone = new MDphone();
            return mDphone.GetPlan(resname);
        }
        public ActionResult<DataTable> Get_mes_state(string planlist)
        {
            //获取所选设备下的工单的状态
            MDphone mDphone = new MDphone();
            return mDphone.GetMesState(planlist);
        }
        public ActionResult<List<string>> BtnReportEvent(string ReportData)
        {
            //按钮的功能
            JObject report_datas = (JObject)JsonConvert.DeserializeObject(ReportData);
            MDphone mDphone = new MDphone();
            return mDphone.BtnReportClick(report_datas);
        }
        public ActionResult<DateTime> WorkOrderStartTime(string resname, string timeType)
        {
            MDphone mDphone = new MDphone();
            return mDphone.GetWorkOrderStartTime(resname, timeType);
        }
        public ActionResult<DataTable> Tag_Event(string res_name)
        {
            //获取已经经过的事件
            MDphone mDphone = new MDphone();
            return mDphone.Get_Tag_Event(res_name);
        }
        public ActionResult<DataTable> Get_Cur_Product(string res_name)
        {
            MDphone mDphone = new MDphone();
            return mDphone.GetCurProduct(res_name);
        }
        public ActionResult<DataTable> GetUserMessage()
        {
            MDphone mDphone = new MDphone();
            return mDphone.UserMessage();
        }
        public ActionResult<DataTable> GetParseMessage() {
            MDphone mDphone = new MDphone();
            return mDphone.ParseMessage();
        }
    }
}