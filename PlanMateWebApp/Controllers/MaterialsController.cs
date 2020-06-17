using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlanMateWebApp.Models;
using PMStaticModels.PlanModels;
using PMStaticModels.Materials;
using PMStaticModels.UserModels;

namespace PlanMateWebApp.Controllers
{
    public class MaterialsController : Controller
    {
        public IActionResult Index()
        {
            //判断empid是否为空,如果为空退出到登陆页面
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
                ViewBag.WorkPlanName = Workplaninfo.WorkPlanName;
                ViewBag.WorkPlanID = Workplaninfo.WorkPlanId;
                ViewBag.Owner = Workplaninfo.Owner;
                ViewBag.ReleaseTime = Workplaninfo.ReleaseTime;
                //获取头部统计信息
                //Materials materials = new Materials();
                //materials.GetMaterialsStatistics();
                //ViewBag.OnstoreQty = materials.OnstoreQty;
                //ViewBag.OnstorePercent = materials.OnstorePercent.ToString("0.00");
                //ViewBag.OnQcQty = materials.OnQcQty;
                //ViewBag.OnQcPercent = materials.OnQcPercent.ToString("0.00");
                //ViewBag.OnVdsQty = materials.OnVdsQty;
                //ViewBag.OnVdsPercent = materials.OnVdsPercent.ToString("0.00");
                //ViewBag.OnNoQty = materials.OnNoQty;
                //ViewBag.OnNoPercenter = materials.OnNoPercent.ToString("0.00");
                return View();
            }
            //获取头部的计划信息
        }
        public string GetMaterialsTable(int choose)
        {
            //获取物料表格
            Materials me = new Materials();
            return me.GetMaterials(choose);
        }
        public IActionResult ArrearsList()
        {
            //欠料表页面
            //判断empid是否为空,如果为空退出到登陆页面
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
                ViewBag.WorkPlanName = Workplaninfo.WorkPlanName;
                ViewBag.WorkPlanID = Workplaninfo.WorkPlanId;
                ViewBag.Owner = Workplaninfo.Owner;
                ViewBag.ReleaseTime = Workplaninfo.ReleaseTime;
                //获取头部统计信息
                //Materials materials = new Materials();
                //materials.GetMaterialsStatistics();
                //ViewBag.OnstoreQty = materials.OnstoreQty;
                //ViewBag.OnstorePercent = materials.OnstorePercent.ToString("0.00");
                //ViewBag.OnQcQty = materials.OnQcQty;
                //ViewBag.OnQcPercent = materials.OnQcPercent.ToString("0.00");
                //ViewBag.OnVdsQty = materials.OnVdsQty;
                //ViewBag.OnVdsPercent = materials.OnVdsPercent.ToString("0.00");
                //ViewBag.OnNoQty = materials.OnNoQty;
                //ViewBag.OnNoPercenter = materials.OnNoPercent.ToString("0.00");
                return View();
            }

        }
    }
}