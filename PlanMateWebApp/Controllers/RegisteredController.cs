using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanMateWebApp.Models;
namespace PlanMateWebApp.Controllers
{
    public class RegisteredController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult<string> EnterRegistered(string data)
        {
            JObject register_data = (JObject)JsonConvert.DeserializeObject(data);
            MDRegistered mDRegistered = new MDRegistered();
            string message = mDRegistered.Set_user(register_data);
            return message;
        }
        public ActionResult<DataTable> All_User(int page)
        {
            DataTable table = new DataTable();
            MDRegistered mDRegistered = new MDRegistered();
            mDRegistered.Get_All_Users(page);
            return table;
        }
    }
}