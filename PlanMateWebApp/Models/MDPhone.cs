using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMSettings;
using PMStaticModels.UserModels;
namespace PlanMateWebApp.Models
{
    public class MDphone
    {
        public ActionResult<DataTable> GetMenu()
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "SELECT resName,ViewName FROM View_PmViewGroup  where sysID = '" + PMUser.UserSysID + "' and VGlobal = 'export'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            return table;
        }
        public ActionResult<DataTable> GetPlan(string resname)
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            //string today = DateTime.Now.ToString("yyyy-MM-dd");
            //string tomorrow = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            string today = "2020-03-30"; //测试数据
            string tomorrow = "2020-04-23"; //测试数据
            DateTime dt = DateTime.Now.AddDays(1);
            cmd.CommandText = "SELECT planStartTime,planendtime,workID,pmOpName,finishedQty,TaskFinishState,itemAttr1,itemAttr2,itemAttr3,itemAttr4,productID,pmResName,jobQty,plannedqty,AllFinishedQty,dayShift FROM User_MesDailyData where pmResName='" + resname + "' and sysID = '" + PMUser.UserSysID + "' and planStartTime >= '" + today+ "' and planStartTime < '"+tomorrow+"' and datatype = 'P' ORDER BY planStartTime";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Dispose();
            return table;
        }
        public ActionResult<DataTable> GetMesState(string planlist)
        {
            //https://localhost:44303/api/RateFactory/GetWorkOrderMesState?workIDList=[{'workid':'workid1','opname':'MI'},{}]
            DataTable MesData = new DataTable();
            DataTable workidlist = JsonConvert.DeserializeObject<DataTable>(planlist.ToString());
            //workidlist  =  workIDList.ToString()
            DataTable workorderstate = new DataTable();
            workorderstate.Columns.Add("workID");
            workorderstate.Columns.Add("opname");
            workorderstate.Columns.Add("state");
            workorderstate.Columns.Add("DayShift");
            workorderstate.Columns.Add("plannedqty");
            workorderstate.Columns.Add("finishqty");
            workorderstate.Columns.Add("BadQty");
            workorderstate.Columns.Add("pmResName");
            SqlCommand cmd = PMCommand.SchCmd(); 
            cmd.CommandText = "select * from wapMesEventRec where MesDate <= '" + DateTime.Now.Date + "' order by PMUID";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(MesData);
            da.Dispose();
            cmd.Connection.Close();
            foreach (DataRow item in workidlist.Rows)
            {
                DataRow[] dr = MesData.Select("workid ='" + item[0].ToString() + "' and Opname = '" + item[1].ToString() + "' and DayShift = '" + item[2].ToString() + "'");
               
                if (dr.Count() > 0)
                {
                    int BadQty = 0;
                    int finishQty = 0;
                    for (int i = 0; i < dr.Count(); i++)
                    {
                        string state = dr[i][2].ToString();
                        BadQty += Convert.ToInt32(dr[i][20]);
                        finishQty += Convert.ToInt32(dr[i][10]);
                        if (state.ToLower() == "startsetup")
                    {
                        DataRow newdr = workorderstate.NewRow();
                        newdr[0] = item[0].ToString();
                        newdr[1] = item[1].ToString();  
                        newdr[2] = "2";
                            newdr[3] = item[2].ToString();
                            newdr[5] = finishQty;
                        newdr[6] = BadQty;
                            newdr[7] = dr[i][4].ToString(); ;
                            workorderstate.Rows.Add(newdr);
                    }
                    else if (state.ToLower() == "endsetup")
                    {
                        DataRow newdr = workorderstate.NewRow();
                        newdr[0] = item[0].ToString();
                        newdr[1] = item[1].ToString();
                        newdr[3] = item[2].ToString();
                        newdr[2] = "3";
                            newdr[5] = finishQty;
                            newdr[6] = BadQty;
                            newdr[7] = dr[i][4].ToString();
                            workorderstate.Rows.Add(newdr);
                    }
                    else if (state.ToLower() == "report")
                    {
                        DataRow newdr = workorderstate.NewRow();
                        newdr[0] = item[0].ToString();
                        newdr[1] = item[1].ToString();
                        newdr[3] = item[2].ToString();
                        newdr[2] = "3";
                            newdr[5] = finishQty;
                            newdr[6] = BadQty;
                            newdr[7] = dr[i][4].ToString();
                            workorderstate.Rows.Add(newdr);
                    }
                    else if (state.ToLower() == "startrest")
                    {
                        DataRow newdr = workorderstate.NewRow();
                        newdr[0] = item[0].ToString();
                        newdr[1] = item[1].ToString();
                        newdr[3] = item[2].ToString();
                        newdr[2] = "5";
                            newdr[5] = finishQty;
                            newdr[6] = BadQty;
                            newdr[7] = dr[i][4].ToString();
                            workorderstate.Rows.Add(newdr);
                    }
                    else if (state.ToLower() == "endrest")
                    {
                        DataRow newdr = workorderstate.NewRow();
                        newdr[0] = item[0].ToString();
                        newdr[1] = item[1].ToString();
                        newdr[3] = item[2].ToString();
                        newdr[2] = "3";
                            newdr[5] = finishQty;
                            newdr[6] = BadQty;
                            newdr[7] = dr[i][4].ToString();
                            workorderstate.Rows.Add(newdr);
                    }
                    else if (state.ToLower() == "startproduct")
                    {
                        DataRow[] finishdr = MesData.Select("EventName ='startproduct' and workid = '" + item[0].ToString() + "' and opname = '" + item[1].ToString() + "' and DayShift = '" + item[2].ToString() + "'");
                        decimal allfinishedQty = 0;
                        decimal planQty = Convert.ToDecimal(finishdr[finishdr.Length - 1]["PlanQty"]);
                            decimal jobQty = 0;
                            allfinishedQty = Convert.ToDecimal(finishdr[finishdr.Length-1]["FinishedQty"]) + Convert.ToDecimal(finishdr[finishdr.Length - 1]["FailedQty"]) + Convert.ToDecimal(finishdr[finishdr.Length - 1]["ScrappedQty"]);
                                jobQty = Convert.ToDecimal(finishdr[finishdr.Length - 1]["JobQty"]);
                                planQty = planQty - allfinishedQty;
                           
                            if (planQty <= allfinishedQty)
                            {
                                DataRow newdr = workorderstate.NewRow();
                                newdr[0] = item[0].ToString();
                                newdr[1] = item[1].ToString();
                                newdr[3] = item[2].ToString();
                                newdr[2] = "4"; 
                                newdr[4] = planQty;
                                newdr[5] = finishQty;
                                newdr[6] = BadQty;
                                newdr[7] = dr[i][4].ToString();
                                workorderstate.Rows.Add(newdr);
                            }
                            else
                            {
                                DataRow newdr = workorderstate.NewRow();
                                newdr[0] = item[0].ToString();
                                newdr[1] = item[1].ToString();
                                newdr[3] = item[2].ToString();
                                newdr[2] = "3";
                                newdr[4] = planQty;
                                newdr[5] = finishQty;
                                newdr[6] = BadQty;
                                newdr[7] = dr[i][4].ToString();
                                workorderstate.Rows.Add(newdr);
                            }
                        }
                    }
                }
            }
            return workorderstate;
        }
        public ActionResult<List<string>> BtnReportClick(JObject report_datas)
        {
            //返回数据的redata说明：
            //row1:执行状态, 0为执行失败，1为执行成功
            //row2:信息列，备注消息详情或者返回数据
            //row3,按钮要变成的状态
            List<string> redata = new List<string>();//q  
            int BtnState = Convert.ToInt32(report_datas["BtnState"]);
            string ResName = report_datas["ResName"].ToString();
            string WorkID = report_datas["WorkID"].ToString();
            string OpName = report_datas["OpName"].ToString();
            string ProductID = report_datas["ProductID"].ToString();
            string Description = report_datas["Description"].ToString();
            string MesStartTime = report_datas["MesStartTime"].ToString();
            string JobQty = report_datas["JobQty"].ToString();
            string FinishedQty = report_datas["FinishedQty"].ToString();
            int BadQty = Convert.ToInt32( report_datas["BadQty"]);
            string ScrappedQty = report_datas["ScrappedQty"].ToString();
            string MesEndTime = report_datas["MesEndTime"].ToString();
            string DayShift = report_datas["dayShift"].ToString();
            string ErrorAttr = report_datas["ErrorAttr"].ToString();
            string PlanQty = report_datas["PlannedQty"].ToString();
            string Operator = PMUser.UserName;
            if (BtnState == 1)
            {
                //执行切换开始操作
                int maxuid = GetTableMaxUID(PMCommand.SchCmd(), "wapMesEventRec", "PMUID");
                SqlCommand cmd = PMCommand.SchCmd();
                cmd.CommandText = "insert into wapMesEventRec (PMUID,EventType,EventName,EventMessage,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,FailedQty,ScrappedQty,MesStartTime,MesEndTime,MesDate,MesOperator,DayShift,PlanQty) " +
                                                       "values ('" + maxuid + "','S','StartSetup','开始生产切换','" + ResName + "','" + WorkID + "','" + OpName + "','" + ProductID + "','" + Description + "','" + JobQty + "','0','0','0','" + DateTime.Now.ToString() + "','" + DateTime.Now.ToString() + "','" + DateTime.Now.Date + "','" + Operator + "','"+ DayShift + "','" + PlanQty + "')";
                int state = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                redata.Add(state.ToString());
                redata.Add("开始生产成功，进入工单切换状态。");
                redata.Add("2");
                redata.Add(Operator);
            }
            else if (BtnState == 2)
            {
                //执行换线结束操作
                //获取当前订单MES开始时间
                SqlCommand cmd = PMCommand.SchCmd();
                //cmd.CommandText = "select messtarttime from wapMesEventRec where  workid = '" + WorkID + "'and opname = '" + OpName + "' and resname = '" + ResName + "' and jobqty = '" + JobQty + "'";
                //SqlDataReader rd = cmd.ExecuteReader();
                //if (rd.Read())
                //{
                //    MesEndTime = rd[0].ToString();
                //}
                //rd.Close();
                MesStartTime = GetWorkOrderStartTime(ResName, "S").Value.ToString();
                int maxuid = GetTableMaxUID(PMCommand.SchCmd(), "wapMesEventRec", "PMUID");
                cmd.CommandText = "insert into wapMesEventRec (PMUID,EventType,EventName,EventMessage,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,FailedQty,ScrappedQty,MesStartTime,MesEndTime,MesDate,MesOperator,DayShift,PlanQty) " +
                                                      "values ('" + maxuid + "','S','EndSetup','结束生产切换','" + ResName + "','" + WorkID + "','" + OpName + "','" + ProductID + "','" + Description + "','" + JobQty + "','0','0','0','" + MesStartTime + "','" + DateTime.Now.ToString() + "','" + DateTime.Now.Date + "','" + Operator + "','" + DayShift + "','" + PlanQty + "')";

                int state = cmd.ExecuteNonQuery();
                ////插入开始生产开始时间
                //maxuid = PMFunc.Func.GetTableMaxUID("sch", "wapMesEventRec", "PMUID");                
                //cmd.CommandText = "insert into wapMesEventRec (PMUID,EventType,EventName,EventMessage,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,FailedQty,ScrappedQty,MesStartTime,MesEndTime,MesDate,MesOperator) " +
                //                                      "values ('" + maxuid + "','S','StartProduct','开始生产','" + ResName + "','" + WorkID + "','" + OpName + "','" + ProductID + "','" + Description + "','" + JobQty + "','0','0','0','" + DateTime.Now.ToString() + "','1900-01-01 0:00:00','" + DateTime.Now.Date + "','" + MesOperator + "')";
                //state = cmd.ExecuteNonQuery();
                ////插入开始生产开始时间
                cmd.Connection.Close();
                redata.Add(state.ToString());
                redata.Add("结束生产切换，进入工单生产状态。");
                redata.Add("3");
                redata.Add(Operator);
            }
            else if (BtnState == 3)
            {
                //执行报工操作
                int maxuid = GetTableMaxUID(PMCommand.SchCmd(), "wapMesEventRec", "PMUID");
                SqlCommand cmd = PMCommand.SchCmd();
                cmd.CommandText = "insert into wapMesEventRec (PMUID,EventType,EventName,EventMessage,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,FailedQty,ScrappedQty,MesStartTime,MesEndTime,MesDate,MesOperator,DayShift,PlanQty) " +
                                                       "values ('" + maxuid + "','P','StartProduct','报工','" + ResName + "','" + WorkID + "','" + OpName + "','" + ProductID + "','" + Description + "','" + JobQty + "','" + FinishedQty + "'," + BadQty + ",'" + ScrappedQty + "','" + MesStartTime + "','" + MesEndTime + "','" + DateTime.Now.Date + "','" + Operator + "','" + DayShift + "','"+ PlanQty + "')";
                int state = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                //报工完成后,判断是否当前订单已经完成
                //如果想知道是否完成,那么应该先知道数据库中这个工单所有的报工数据
                //cmd.CommandText = "select * from wapMesEventRec where workid = '" + WorkID + "' and  opname = '" + OpName + "'and EventName = 'StartProduct'";
                //DataTable MesData = new DataTable();
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //da.Fill(MesData);
                //da.Dispose();
                //cmd.Connection.Close();
                //decimal allfinishedQty = 0;
                //if (MesData.Rows.Count > 0)
                //{
                //    foreach (DataRow item in MesData.Rows)
                //    {
                //        allfinishedQty += Convert.ToDecimal(item["FinishedQty"]) + Convert.ToDecimal(item["FailedQty"]) + Convert.ToDecimal(item["ScrappedQty"]);
                //    }
                //}
                //if (allfinishedQty >= Convert.ToDecimal(JobQty))
                //{
                //    redata.Add(state.ToString());
                //    redata.Add("报工成功。");
                //    redata.Add("4");
                //}
                //else
                //{
                //    redata.Add(state.ToString());
                //    redata.Add("报工成功。");
                //    redata.Add("3");
                //}

                if (Convert.ToInt32(PlanQty) == Convert.ToInt32(FinishedQty)+Convert.ToInt32(BadQty)+Convert.ToInt32(ScrappedQty))
                {
                    redata.Add(state.ToString());
                    redata.Add("报工成功。");
                    redata.Add("4");
                    redata.Add(Operator);
                }
                else
                {
                        redata.Add(state.ToString());
                        redata.Add("报工成功。");
                        redata.Add("3");
                    redata.Add(Operator);
                }
            }
            else if (BtnState == 4)
            {
                //执行工单完结
            }
            else if (BtnState == 5)
            {
                //执行工单继续生产(工单暂停的事件)
                //执行切换开始操作
                SqlCommand cmd = PMCommand.SchCmd();
                //cmd.CommandText = "select messtarttime from wapMesEventRec where  workid = '" + WorkID + "'and opname = '" + OpName + "' and resname = '" + ResName + "' and jobqty = '" + JobQty + "'";
                //SqlDataReader rd = cmd.ExecuteReader();
                //if (rd.Read())
                //{
                //    MesEndTime = rd[0].ToString();
                //}
                //rd.Close();
                int maxuid = GetTableMaxUID(PMCommand.SchCmd(), "wapMesEventRec", "PMUID");
                cmd.CommandText = "insert into wapMesEventRec (PMUID,EventType,EventName,EventMessage,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,FailedQty,ScrappedQty,MesStartTime,MesEndTime,MesDate,MesOperator,DayShift,ErrorAttr,PlanQty) " +
                                                      "values ('" + maxuid + "','S','StartRest','暂停生产','" + ResName + "','" + WorkID + "','" + OpName + "','" + ProductID + "','" + Description + "','" + JobQty + "','0','0','0','" + "" + "','','','" + Operator + "','" + DayShift + "','"+ ErrorAttr + "','" + PlanQty + "')";

                int state = cmd.ExecuteNonQuery();
                ////插入开始生产开始时间
                //maxuid = PMFunc.Func.GetTableMaxUID("sch", "wapMesEventRec", "PMUID");                
                //cmd.CommandText = "insert into wapMesEventRec (PMUID,EventType,EventName,EventMessage,ResName,WorkID,OpName,ProductID,Description,JobQty,FinishedQty,FailedQty,ScrappedQty,MesStartTime,MesEndTime,MesDate,MesOperator) " +
                //                                      "values ('" + maxuid + "','S','StartProduct','开始生产','" + ResName + "','" + WorkID + "','" + OpName + "','" + ProductID + "','" + Description + "','" + JobQty + "','0','0','0','" + DateTime.Now.ToString() + "','1900-01-01 0:00:00','" + DateTime.Now.Date + "','" + MesOperator + "')";
                //state = cmd.ExecuteNonQuery();
                ////插入开始生产开始时间
                cmd.Connection.Close();
                redata.Add(state.ToString());
                redata.Add("暂停生产。");
                redata.Add("5");
                redata.Add(Operator);
            }
            else
            {
                redata.Add("System Error");
                redata.Add("Button state is wrong!");
            }
            return redata;
        }
        public int GetTableMaxUID(SqlCommand cmd, string TableName, string UIDName)
        {
            int maxuid = 0;
            cmd.CommandText = "select MAX(" + UIDName + ") as maxuid  from " + TableName;
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            //string aaa = reader[0].ToString();
            if (!string.IsNullOrEmpty(reader[0].ToString()))
            {
                maxuid = Convert.ToInt32(reader[0]) + 1;
            }
            else
            {
                maxuid = 1;
            }
            reader.Close();
            cmd.Connection.Close();
            return maxuid;
        }
        public ActionResult<DateTime> GetWorkOrderStartTime(string resname, string timeType)
        {
            string colName;
            if (timeType.ToUpper() == "S")
            {
                colName = "MesStartTime";
            }
            else
            {
                colName = "MesEndTime";
            }
            DateTime starttime;
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select " + colName + " from wapMesEventRec where resname = '" + resname + "' order by mesendtime DESC";
            SqlDataReader rd = cmd.ExecuteReader();
            rd.Read();
            starttime = Convert.ToDateTime(rd[0]);
            rd.Close();
            cmd.Connection.Close();
            return starttime;
        }
        public ActionResult<DataTable> Get_Tag_Event(string res_name)
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select * from wapMesEventRec where ResName = '"+ res_name + "' and MesDate <= '" + DateTime.Now.Date + "' order by PMUID desc";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Close();
            return table;
        }
        public ActionResult<DataTable> GetCurProduct(string res_name)
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select * from wapMesEventRec where ResName='" + res_name+ "' and EventName='EndSetup'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Close();
            return table;
        }
        public ActionResult<DataTable> UserMessage()
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.ModCmd();
            cmd.CommandText = "select empName,phoneNum,sysID,email from wapEmpList where empID='" + PMUser.EmpID + "'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Close();
            return table;
        }
        public ActionResult<DataTable> ParseMessage()
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PMCommand.SchCmd();
            cmd.CommandText = "select eventType from wapEventRep";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Close();
            return table;
        }
    }
}
