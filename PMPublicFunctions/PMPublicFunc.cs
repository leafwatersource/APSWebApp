using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using PMSettings;
using PMStaticModels.UserModels;

namespace PMPublicFunctions.PMPublicFunc
{
    public static class PMPublicFuncs
    {
        public static void WriteLogs(string empID, string empName, string ipaddress, string model, DateTime time, string message, string webinfo)
        {
            //写入log
            SqlCommand cmd =PMCommand.CtrlCmd();
            cmd.Parameters.Add("@empID", SqlDbType.VarChar).Value = empID;
            cmd.Parameters.Add("@empName", SqlDbType.VarChar).Value = empName;
            cmd.Parameters.Add("@ipaddress", SqlDbType.VarChar).Value = ipaddress;
            cmd.Parameters.Add("@model", SqlDbType.VarChar).Value = model;
            cmd.Parameters.Add("@time", SqlDbType.DateTime).Value = time;
            cmd.Parameters.Add("@message", SqlDbType.VarChar).Value = message;
            cmd.Parameters.Add("@webinfo", SqlDbType.VarChar).Value = webinfo;
            cmd.CommandText = "insert into wapUserlog (empID,empName,ipAddress,model,logtime,logmessage,webinfomation) values (@empID,@empName,@ipaddress,@model,@time,@message,@webinfo)";
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        public static string DatatableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
                return JsonString.ToString();
            }
            else
            {
                return string.Empty;
            }           
        }

        public static string ListToJson<T>(List<T> list)
        {
            if (list == null || list.Count < 1)
            {
                return string.Empty;
            }
            else
            {
                StringBuilder jsonBud = new StringBuilder();
                jsonBud.Append("[");
                for (int i = 0; i < list.Count; i++)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                    MemoryStream ms = new MemoryStream();
                    ser.WriteObject(ms, list[i]);
                    StreamReader reader = new StreamReader(ms);
                    ms.Position = 0;
                    string str = reader.ReadToEnd();
                    reader.Close();
                    ms.Close();
                    jsonBud.Append(str);
                    jsonBud.Append(",");
                }
                jsonBud.Remove(jsonBud.Length - 1, 1);
                jsonBud.Append("]");
                
                return jsonBud.ToString();
            }           
        }

        public static string GetMd5(string str)
        {
            MD5 md5str = MD5.Create();
            byte[] s = md5str.ComputeHash(Encoding.UTF8.GetBytes(str));
            md5str.Dispose();
            return Convert.ToBase64String(s);
        }

        public static string GetCookieValue(HttpContext http, string key)
        {
            http.Request.Cookies.TryGetValue(key, out string value);
            return value;
        }

        public static void DeleteUser()
        {
            SqlCommand cmd = PMCommand.CtrlCmd();
            cmd.CommandText = "delete from wapUserstate where Empid = '" + PMUser.EmpID + "'";
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            WriteLogs(PMUser.EmpID, PMUser.UserName, PMUser.UserIpAdress, "用户退出", DateTime.Now, "用户正常退出。",PMUser.UserWeb);
            PMUser.EmpID = string.Empty;
            PMUser.UserPass = string.Empty;
            PMUser.UserIpAdress = string.Empty;
            PMUser.UserWeb = string.Empty;
            PMUser.UserName = string.Empty;
            PMUser.UserGuid = string.Empty;
        }
       
    }
}
