using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMStaticModels.UserModels;
using PMPublicFunctions.PMPublicFunc;

namespace PlanMateWebApp.Controllers
{
    public class SelectorController : Controller
    {
        public IActionResult Index()
        {
            if (PMUser.EmpID == String.Empty)
            {
                return RedirectToAction("Index", "Index");
            }
            else if(HttpContext.Request.Cookies["UserGuid"] != PMUser.GetuserGuid(PMUser.EmpID))
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                return View();
            }            
        }
        public bool IsAdmin()
        {
            if (HttpContext.Request.Cookies["MD5"] != null)
            {
                return true;
            }
            else
            {
                return false;                
            }
        }
        public string FunctionResult()
        {
            return PMPublicFuncs.ListToJson(PMUser.FunctionList);
        }
    }
}