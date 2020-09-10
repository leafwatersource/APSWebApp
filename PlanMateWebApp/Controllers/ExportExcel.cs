using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;
//using Excel = Microsoft.Off
namespace PlanMateWebApp.Controllers
{
    public class ExportExcel
    {
        /// <summary>
        /// 创建excel文件并写入数据,生成的文件在wwwroot目录下StaticExcel下
        /// </summary>
        /// <param name="dt">DataTable数据</param>
        /// <param name="FileName">文件名称test.xlsx</param>
        /// <param name="sheetNames">Sheet的名称</param>
        /// <returns>返回生成excel的路径</returns>
        public static string Excel(DataTable dt,string FileName,string sheetNames)
        {
            string sWebRootFolder = PMAppSettings.BasePath+ @"wwwroot\StaticExcel\";
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, FileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, FileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetNames);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[1, (i + 1)].Value = dt.Columns[i].ColumnName;
                    worksheet.Cells[1, (i+1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, (i+1)].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 123, 255));
                    worksheet.Cells[1, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(0, 0, 0));
                    worksheet.Cells[1, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, (i + 1)].Style.Font.Color.SetColor(Color.White);
                    worksheet.Cells[1, (i + 1)].Style.Font.Size = 9;
                    worksheet.Cells[1, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                    worksheet.Row(1).CustomHeight = true;
                    worksheet.Column(i+1).AutoFit();
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    worksheet.Row(i + 2).CustomHeight = true;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        worksheet.Cells[(i + 2), (j + 1)].Value = dt.Rows[i][j].ToString();
                        worksheet.Cells[(i + 2), (j + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(191, 191, 191));
                        worksheet.Cells[(i + 2), (j + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[(i + 2), (j + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(0, 0, 0));
                        worksheet.Cells[(i + 2), (j + 1)].Style.Font.Size = 9;
                        worksheet.Cells[(i + 2), (j + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                        worksheet.Row(i + 2).CustomHeight = true;//自动调整行高
                    }
                }
                worksheet.Cells.Style.ShrinkToFit = true;
                try
                {
                    package.Save();
                }
                catch (Exception)
                {
                }
            }
            return @"\StaticExcel\"+FileName;
        }
        /// <summary>
        /// 删除wwwroot\StaticExcel\目录下的指定文件
        /// </summary>
        /// <param name="FileName">文件的名称,不用带后缀</param>
        public static void DelExcel(string FileName)
        {
            foreach (string d in Directory.GetFileSystemEntries(PMAppSettings.BasePath+ @"wwwroot\StaticExcel\"))
            {
                if (File.Exists(d))
                {
                    string name = Path.GetFileNameWithoutExtension(d);
                    if (FileName == name)
                    {
                        File.Delete(d);
                        break;
                    }
                }
            }

        }
    }
}
