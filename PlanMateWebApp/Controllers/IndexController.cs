using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMStaticModels.UserModels;
using PMPublicFunctions.PMPublicFunc;
using PlanMateWebApp.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Security.Cryptography;

namespace PlanMateWebApp.Controllers
{
    
    public class IndexController : Controller
    {
        MLogin login;
         //[Route("Index")]
        // [Route("Index/Index")]
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult<LoginMessage> Login(string empID, string pwd, string adminstate)
        {
            if(login == null)
            {
                login = new MLogin();
            }
            MD5 md5 = MD5.Create();
            //PMStaticModels.UserModels.PMUser.UserSysID
            pwd += empID;
            string userPass = "";
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(pwd.Trim()));
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                userPass += s[i].ToString("X");
            }
            User user = new User();
            user.EmpID = empID;
            user.UserPass = userPass;
            user.UserIpAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            user.UserWeb = Request.Headers["User-Agent"];

            PMUser.EmpID = empID;
            PMUser.UserPass = userPass;
            PMUser.UserIpAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            PMUser.UserWeb = Request.Headers["User-Agent"];
            LoginMessage userMsg = login.LoginMessage();
            if (userMsg.LoginState == "1")
            {
                List<string> userGroup = login.GetuserGroup(empID);
                if (userGroup.Count < 1)
                {
                    userMsg.LoginState = "0";
                    userMsg.Message = "该员工没有分配用户组，请联系管理员分配。";
                }
                else
                {
                    user.UserGuid = userMsg.UserGuid;
                    user.UserName = login.GetempName(empID);

                    PMUser.UserGuid = userMsg.UserGuid;
                    PMUser.UserName = login.GetempName(empID);
                    Response.Cookies.Append("EmpID", user.EmpID);
                    //Response.Cookies.Append("EmpID", PMUser.EmpID);
                    Response.Cookies.Append("UserGuid", PMUser.UserGuid);
                }

                if (adminstate == "1")
                {
                    if (userGroup.Contains("ADMIN") == false)
                    {
                        userMsg.LoginState = "0";
                        userMsg.Message = "请不要使用非管理员账户越权操作!";
                        PMPublicFuncs.WriteLogs(empID, login.GetempName(empID), PMUser.UserIpAdress, "越权登陆", DateTime.Now, "用户越权使用管理员登陆。", PMUser.UserWeb);
                    }
                    else
                    {
                        string md5Guid = Guid.NewGuid().ToString();
                        Response.Cookies.Append("MD5", PMPublicFuncs.GetMd5("ADMIN" + md5Guid));
                        //Response.Cookies.Append("MD5", PMPublicFuncs.GetMd5("ADMIN" + md5Guid), new CookieOptions() { IsEssential = true });
                        PMPublicFuncs.WriteLogs(empID, login.GetempName(empID), PMUser.UserIpAdress, "管理员登录", DateTime.Now, "管理员登陆成功。", PMUser.UserWeb);
                        //管理员登录成功
                    }
                }
                else
                {
                    //判断该用户具有的功能模块权限，如果只有一个权限，直接跳入页面，如果有多个权限，给出选择
                    if (PMUser.FunctionList == null)
                    {
                        PMUser.FunctionList = new List<string>();
                    }
                    PMUser.FunctionList.Clear();
                    foreach (string item in userGroup)
                    {
                        if (item == "ADMIN")
                        {
                            continue;
                        }
                        else if (item == "CFM")
                        {
                            PMUser.FunctionList.Add("systemsetting");
                        }
                        else if (item == "REP")
                        {
                            PMUser.FunctionList.Add("reportsystem");
                        }
                        else if (item == "VIEW")
                        {
                            PMUser.FunctionList.Add("datacenter");
                        }
                        else if (item == "BOARD")
                        {
                            PMUser.FunctionList.Add("planboard");
                        }
                    }
                    PMUser.UserMessage.Add(user);
                    //登录成功
                    PMPublicFuncs.WriteLogs(empID, login.GetempName(empID), PMUser.UserIpAdress, "用户登陆", DateTime.Now, "用户登陆成功。", PMUser.UserWeb);
                }
            }
            return userMsg;
        }

        public ActionResult<LoginMessage> Logout(string empID, string userPass, string adminstate)
        {
            if (login == null)
            {
                login = new MLogin();
            }
            PMUser.EmpID = empID;
            PMUser.UserPass = userPass;
            PMUser.UserIpAdress = HttpContext.Connection.LocalIpAddress.ToString();
            PMUser.UserWeb = Request.Headers["User-Agent"];
            if (login.ForceLogout(empID) == 1)
            {
                return Login(empID, userPass, adminstate);
            }
            else
            {
                LoginMessage errMessage = new LoginMessage();
                errMessage.LoginState = "0";
                errMessage.Message = "强制退出失败，请联系管理员。";
                errMessage.EmpID = login.GetempName(empID);
                return errMessage;
            }
        }
    }
}