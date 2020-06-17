using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using PMSettings;
using System.Security.Cryptography;
using System.Text;
using PMStaticModels.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace PlanMateWebApp.Models
{
    public class MDRegistered
    {
        public int Get_user(string username)
        {
            SqlCommand cmd = PMCommand.ModCmd();
            cmd.CommandText = "select count(*) from wapEmpList where empID = '"+username+"'";
            int counts = (int)cmd.ExecuteScalar();
            cmd.Connection.Dispose();
            return counts;
        }
        public string Set_wapUser_purview(JObject data) {
            //设置用户的权限
            string username = data["username"].ToString();
            string wapUserStr = "";
            string wapEmpUserMapStr = "";
            if (!String.IsNullOrEmpty(data["ADMIN"].ToString()))
            {
                //用户有这个权限走里面的方法
                wapUserStr+= wapUserStr == ""?"('admin_"+username+ "','ADMIN','" + PMUser.UserSysID + "')" : ",('admin_" + username + "','ADMIN','"+ PMUser.UserSysID + "')";
                wapEmpUserMapStr += wapEmpUserMapStr == "" ? "admin_"+username : ",admin_" + username;
            }
            if (!String.IsNullOrEmpty(data["BOARD"].ToString()))
            {
                wapUserStr += wapUserStr == "" ? "('board_" + username + "','BOARD','" + PMUser.UserSysID + "')" : ",('board_" + username + "','BOARD','" + PMUser.UserSysID + "')";
                wapEmpUserMapStr += wapEmpUserMapStr == "" ? "board_" + username : ",board_" + username;
            }
            if (!String.IsNullOrEmpty(data["REP"].ToString()))
            {
                wapUserStr += wapUserStr == "" ? "('rep_" + username + "','REP','" + PMUser.UserSysID + "')" : ",('rep_" + username + "','REP','" + PMUser.UserSysID + "')";
                wapEmpUserMapStr += wapEmpUserMapStr == "" ? "rep_" + username : ",rep_" + username;
            }
            if (!String.IsNullOrEmpty(data["VIEW"].ToString()))
            {
                wapUserStr += wapUserStr == "" ? "('view_" + username + "','VIEW','" + PMUser.UserSysID + "')" : ",('view_" + username + "','VIEW','" + PMUser.UserSysID + "')";
                wapEmpUserMapStr += wapEmpUserMapStr == "" ? "view_" + username : ",view_" + username;
            }
            if (!String.IsNullOrEmpty(data["CFM"].ToString()))
            {
                wapUserStr += wapUserStr == "" ? "('cfm_" + username + "','CFM','" + PMUser.UserSysID + "')" : ",('cfm_" + username + "','CFM','" + PMUser.UserSysID + "')";
                wapEmpUserMapStr += wapEmpUserMapStr == "" ? "cfm_" + username : ",cfm_" + username;
            }
            SqlCommand cmd = PMCommand.ModCmd();
            cmd.CommandText = "insert into wapUser(userName,shopUserGroupID,sysID) values " + wapUserStr;
            cmd.ExecuteNonQuery();
            cmd.Connection.Dispose();
            return this.Set_user_purview(username, wapEmpUserMapStr);
        }
        public string Set_user_purview(string empid,string purviewStr) {
            string[] purviewList = purviewStr.Split(",");
            string purviewListstr = String.Empty;
            foreach (var item in purviewList)
            {
                if (String.IsNullOrEmpty(purviewListstr))
                {
                    purviewListstr += "('" + empid + "','" + item + "')";
                }
                else
                {
                    purviewListstr += ",('" + empid + "','" + item + "')";

                }
            }
            SqlCommand cmd = PMCommand.ModCmd();
            cmd.CommandText = "insert into wapEmpUserMap(empID,userName) values " + purviewListstr;
            int count = cmd.ExecuteNonQuery();
            cmd.Connection.Dispose();
            if (count>=1)
            {
                return "用户添加成功";
            }
            else
            {
                return "用户添加失败";
            }

        }
        public void Add_user(JObject data)
        {
            //增加用户
            string username = data["username"].ToString().Trim();
            string pwd = data["password"].ToString().Trim();
            string name = data["name"].ToString().Trim();
            string phone = data["phone"].ToString().Trim();
            string email = data["email"].ToString().Trim();
            MD5 md5 = MD5.Create();
            //PMStaticModels.UserModels.PMUser.UserSysID
            pwd += username;
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            string password = "";
      
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                password += s[i].ToString("X");
            }
            SqlCommand cmd = PMCommand.ModCmd();
            cmd.CommandText = "insert into wapEmpList(empID,empName,password,phoneNum,email,sysID) values('" + username+ "','" + name + "','" + password+"','"+ phone + "','"+email+"','"+ PMUser.UserSysID + "')";
            cmd.ExecuteScalar();
            cmd.Connection.Dispose();
        }
        public string Set_user(JObject data)
        {
            string username = data["username"].ToString();
            int counts = this.Get_user(username);
            if (counts!=0)
            {
                return "该账户已存在";
            }
            else
            {
                this.Add_user(data);
                return this.Set_wapUser_purview(data);
            }
        }
    
        //查看用户
        public ActionResult<DataTable> Get_All_Users(int count)
        {
            DataTable table = new DataTable();
            //string[] Authority = null;
            if (count == 1)
            {
                SqlCommand cmd = PMCommand.ModCmd();
                cmd.CommandText = "select top 10 empID,empName,phoneNum,email  from wapEmpList";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(table);
                da.Dispose();
                cmd.Connection.Dispose();
            }
            else
            {
                SqlCommand cmd = PMCommand.ModCmd();
                cmd.CommandText = "declare @m int = "+count+ "; declare @n int = 8; select top(@n) empID,empName,phoneNum,email from wapEmpList where empID not in(select top(@m - 1) empID from wapEmpList)";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(table);
                da.Dispose();
                cmd.Connection.Dispose();
            }
            //for (int i = 0; i < table.Rows.Count; i++)
            //{
            //    Authority.Append(table.Rows[i]["empID"].ToString());
            //}
            return table;
        }
    }
}
