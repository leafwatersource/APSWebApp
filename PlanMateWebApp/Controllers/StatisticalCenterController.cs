using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlanMateWebApp.Models;
using PMStaticModels.PlanModels;
using PMStaticModels.StatisticalModels;
using PMStaticModels.UserModels;

namespace PlanMateWebApp.Controllers
{
    public class StatisticalCenterController : Controller
    {
        /// <summary>
        /// 每日产出视图方法
        /// </summary>
        /// <returns>View</returns>
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
                //获取数据
                //头部计划信息
                ViewBag.WorkPlanName = Workplaninfo.WorkPlanName;
                ViewBag.Owner = Workplaninfo.Owner;
                ViewBag.ReleaseTime = Workplaninfo.ReleaseTime;
                return View();
            }
        }
        public IActionResult CenterProcess()
        {
            //中间工序的页面
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
                //获取数据
                //头部计划信息
                ViewBag.WorkPlanName = Workplaninfo.WorkPlanName;
                ViewBag.Owner = Workplaninfo.Owner;
                ViewBag.ReleaseTime = Workplaninfo.ReleaseTime;
                return View();
            }
        }
        /// <summary>
        /// 获取每日产出数据,供AJAX调用
        /// </summary>
        /// <param name="isfinal">是否为最后工序,0为中间工序,1为最后工序,其他为全部工序</param>
        /// <param name="OpName">工序名称</param>
        /// <returns>View</returns>
        public string GetProductOutput(int isfinal, string OpName)
        {
            MDStatisticalCenter sta = new MDStatisticalCenter();
            return sta.ProductOutput(isfinal, OpName);
        }

        /// <summary>
        /// 获取工序名称,供AJAX调用
        /// </summary>
        /// <returns></returns>
        public string GetopName()
        {
            MDStatisticalCenter mDStatisticalCenter = new MDStatisticalCenter();
            return mDStatisticalCenter.GetOpName();
        }

        /// <summary>
        /// 设备稼动率视图方法
        /// </summary>
        /// <returns></returns>
        public IActionResult AuxiliaryResources()
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
                //获取数据
                //头部计划信息
                ViewBag.WorkPlanName = Workplaninfo.WorkPlanName;
                ViewBag.Owner = Workplaninfo.Owner;
                ViewBag.ReleaseTime = Workplaninfo.ReleaseTime;
                return View();
            }
        }
        /// <summary>
        /// 获取设备组方法 供AJAX调用
        /// </summary>
        /// <returns>Json</returns>
        public string GetResGroup()
        {
            MDResUsuage res = new MDResUsuage();
            return res.GetResGroup();
        }

        /// <summary>
        /// 获取每个设备组中的设备列表 供AJAX调用
        /// </summary>
        /// <param name="resGroup"></param>
        /// <returns>Json</returns>
        public string GetResNameList(string resGroup)
        {
            MDResUsuage res = new MDResUsuage();
            return res.GetResNameList(resGroup);
        }

        /// <summary>
        /// 获取设备的设备负载率 供AJAX调用
        /// </summary>
        /// <param name="resName">设备名称</param>
        /// <returns>Json</returns>
        public string GetResUsuage(string resName)
        {
            MDResUsuage res = new MDResUsuage();
            return res.GetResUsuage(resName);
        }
        public ActionResult<List<ResGroup>> GetAllResUsuage()
        {
            MDResUsuage res = new MDResUsuage();
            return res.GetAllUsuage();
        }
        public ActionResult<DataTable> GetuseDate()
        {
             MDResUsuage res = new MDResUsuage();
            return res.GetuseDate();
        }
        /// <summary>
        /// 辅助资源使用视图
        /// </summary>
        /// <returns>View</returns>
        public IActionResult ScndUsuage()
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
                //获取数据
                //头部计划信息
                ViewBag.WorkPlanName = Workplaninfo.WorkPlanName;
                ViewBag.Owner = Workplaninfo.Owner;
                ViewBag.ReleaseTime = Workplaninfo.ReleaseTime;
                return View();
            }
        }
        /// <summary>
        /// 获取辅助资源数据 供AJAX调用
        /// </summary>
        /// <returns>Json</returns>
        public string GetScndUsuage()
        {
            MDScndUsuage mDStatistical = new MDScndUsuage();
            return mDStatistical.GetScndUsuageData();
        }
    }
}