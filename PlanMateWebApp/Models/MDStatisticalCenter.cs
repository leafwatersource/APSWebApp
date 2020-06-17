using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using PMStaticModels.StatisticalModels;
using PMPublicFunctions.PMPublicFunc;
using Microsoft.AspNetCore.Mvc;
using PMSettings;
namespace PlanMateWebApp.Models
{
    public class ResGroup
    {
        private string ResName;
        private DataTable ResContent;
        public string Resname {
            set { ResName = value; }
            get { return ResName; }
        }
        public DataTable Rescontent
        {
            set { ResContent = value; }
            get { return ResContent; }
        }
    }
    public class MDStatisticalCenter
    {
        public string ProductOutput(int isfinal, string opname)
        {
            //Finished参数判断是否是成品
            //获取每日产出数据
            DataTable temptable = StatisticalModels.GetProductOutPut(isfinal, opname);
            //获取工序名称
            //汉化
            temptable.Columns["itemname"].ColumnName = "产品名称";
            temptable.Columns["outputqty"].ColumnName = "产出数量";
            temptable.Columns["exportdate"].ColumnName = "产出日期";
            if (isfinal == 0)
            {
               temptable.Columns["opName"].ColumnName = "工序名称";
            }
            for (int i = 0; i < Convert.ToInt32( PMAppSettings.ItemAttrCount); i++)
            {
                string temp = "itemattr" + (i + 1);
                string item = "ItemAttr"+(i+1);
                temptable.Columns[temp].ColumnName = PMAppSettings.ItemGroup.Rows[0][item].ToString();
            }
            temptable.Columns["desp"].ColumnName = "描述";
            return PMPublicFuncs.DatatableToJson(temptable);
        }
        public string GetOpName()
        {
            DataTable temptable = StatisticalModels.GetProductOutPut(0, string.Empty);
            //获取工序名称

            List<string> op = new List<string>();
            foreach (DataRow item in temptable.Rows)
            {
                if (!op.Contains(item["opName"].ToString()))
                {
                    op.Add(item["opName"].ToString());
                }
                else
                {
                    continue;
                }
            }
            StatisticalModels.OpName = PMPublicFuncs.ListToJson(op);
            return StatisticalModels.OpName;
        }

    }

    /// <summary>
    /// 设备负载率的类
    /// </summary>
    public class MDResUsuage
    {
        /// <summary>
        /// 获取设备组
        /// </summary>
        /// <returns>Json</returns>
        public string GetResGroup()
        {
            return PMPublicFuncs.DatatableToJson(ResourceModels.GetResGroup());
        }
        /// <summary>
        /// 获取设备名称列表
        /// </summary>
        /// <param name="resGroup">设备组名称</param>
        /// <returns>Json</returns>
        public string GetResNameList(string resGroup)
        {
            return PMPublicFuncs.DatatableToJson(ResourceModels.GetResList(resGroup));
        }
        /// <summary>
        /// 获取设备利用率
        /// </summary>
        /// <param name="resName">线体名称</param>
        /// <returns>Json</returns>
        public string GetResUsuage(string resName)
        {
            DataTable data = ResourceModels.GetResUsuage(resName);
            //useDate,dayAllWorkHour,dayPlanWorkHour,dayResRatio
            data.Columns["useDate"].ColumnName = "日期";
            data.Columns["dayAllWorkHour"].ColumnName = "当日总工时";
            data.Columns["dayPlanWorkHour"].ColumnName = "计划工时";
            data.Columns["dayResRatio"].ColumnName = "稼动率";
            return PMPublicFuncs.DatatableToJson(data);
        }
        public  ActionResult<List<ResGroup>> GetAllUsuage()
        {
            List<ResGroup> resGroups = new List<ResGroup>();
            ResGroup resGroup = new ResGroup();
            DataTable data = ResourceModels.GetAllResUsuage();
            resGroup.Rescontent = new DataTable();
            resGroup.Rescontent.Columns.Add("日期");
            resGroup.Rescontent.Columns.Add("当日总工时");
            resGroup.Rescontent.Columns.Add("计划工时");
            resGroup.Rescontent.Columns.Add("稼动率");
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (i==0)
                {
                    resGroup.Resname = data.Rows[i]["mainResName"].ToString();
                    DataRow row = resGroup.Rescontent.NewRow();
                    row["日期"] = data.Rows[i]["useDate"].ToString();
                    row["当日总工时"] = data.Rows[i]["dayAllWorkHour"].ToString();
                    row["计划工时"] = data.Rows[i]["dayPlanWorkHour"].ToString();
                    row["稼动率"] = data.Rows[i]["dayResRatio"].ToString();
                    resGroup.Rescontent.Rows.Add(row);
                    continue;
                }
                if (data.Rows[i]["mainResName"].ToString() == resGroup.Resname.ToString())
                {
                    DataRow row = resGroup.Rescontent.NewRow();
                    row["日期"] = data.Rows[i]["useDate"].ToString();
                    row["当日总工时"] = data.Rows[i]["dayAllWorkHour"].ToString();
                    row["计划工时"] = data.Rows[i]["dayPlanWorkHour"].ToString();
                    row["稼动率"] = data.Rows[i]["dayResRatio"].ToString();
                    resGroup.Rescontent.Rows.Add(row);
                }
                else
                {
                    resGroups.Add(resGroup);
                    resGroup = new ResGroup();
                    resGroup.Resname = data.Rows[i]["mainResName"].ToString() ;
                    resGroup.Rescontent = new DataTable();
                    DataRow row = resGroup.Rescontent.NewRow();
                    resGroup.Rescontent.Columns.Add("日期");
                    resGroup.Rescontent.Columns.Add("当日总工时");
                    resGroup.Rescontent.Columns.Add("计划工时");
                    resGroup.Rescontent.Columns.Add("稼动率");
                    row["日期"] = data.Rows[i]["useDate"].ToString();
                    row["当日总工时"] = data.Rows[i]["dayAllWorkHour"].ToString();
                    row["计划工时"] = data.Rows[i]["dayPlanWorkHour"].ToString();
                    row["稼动率"] = data.Rows[i]["dayResRatio"].ToString();
                    resGroup.Rescontent.Rows.Add(row);
                }
            }
            return resGroups;
        }
        public ActionResult<DataTable> GetuseDate()
        {
            DataTable data = ResourceModels.GetUseDate();
            return data;
        }
    }

    /// <summary>
    /// 辅助资源利用类
    /// </summary>
    public class MDScndUsuage
    {
        /// <summary>
        /// 从静态模型中获取辅助资源数据
        /// </summary>
        /// <returns>Json</returns>
        public string GetScndUsuageData()
        {
            DataTable data = ScndUsuageModels.GetScndUsuageData();
            //scndResName,scndResType,allQty,useQty,startDateTime,endDateTime
            data.Columns["scndResName"].ColumnName = "辅助资源名";
            data.Columns["scndResType"].ColumnName = "辅助资源类型";
            data.Columns["allQty"].ColumnName = "可用总数";
            data.Columns["useQty"].ColumnName = "使用数量";
            data.Columns["startDateTime"].ColumnName = "使用开始时间";
            data.Columns["endDateTime"].ColumnName = "使用结束时间";
            return PMPublicFuncs.DatatableToJson(data);
        }
    }
}
