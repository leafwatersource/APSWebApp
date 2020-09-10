using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PlanMateWebApp.Models
{
    public class MDHistory
    {
        public JObject MGetHistoryData()
        {
            JObject obj = PMAppSettings.TableFileds.SelectToken("HistoryTableFiled").ToObject<JObject>();
            string sqlStr = "";
            DataTable table = new DataTable();
            foreach (var item in obj)
            {
                if (string.IsNullOrEmpty(sqlStr))
                {
                    sqlStr += item.Key;
                }
                else
                {
                    sqlStr += "," + item.Key;
                }
            }
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "SELECT " + sqlStr + " from wapMesEventRec order by EventTime desc";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            foreach (var item in obj)
            {
               table.Columns[item.Key].ColumnName = item.Value.Value<string>();
            }
            JObject tableData = new JObject {
                { "total", table.Rows.Count },
                { "rows", JsonConvert.SerializeObject(table) }
            };
            //tableData.Add("total", dt.Rows.Count);
            //tableData.Add("rows", JsonConvert.SerializeObject(dt));
            JObject data = new JObject {
                { "code", "0"},
                { "data", tableData},
                { "msg", "successful"}
            };
            return data;
        }
    }
}
