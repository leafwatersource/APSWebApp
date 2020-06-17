using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlanMateWebApp.Models;
using PMPublicFunctions.PMPublicFunc;

namespace PlanMateWebApp.Controllers
{
    public class AdministratorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Usersetting()
        {
            return View();
        }
        public string GetLog()
        {
            //获取日志,返回数据
            Loglist list = new Loglist();
            return PMPublicFuncs.DatatableToJson(list.Getlist());

        }
    }
}