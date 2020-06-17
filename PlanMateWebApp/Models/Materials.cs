using PMPublicFunctions.PMPublicFunc;
using PMSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using PMStaticModels.Materials;

namespace PlanMateWebApp.Models
{
    public class Materials
    {
        //在库库存
        public decimal OnstoreQty { get; set; }
        public decimal OnstorePercent{ get; set; }
        //在途库存
        public decimal OnVdsQty{ get; set; }
        public decimal OnVdsPercent { get; set; }
        //在检库存
        public decimal OnQcQty { get; set; }
        public decimal OnQcPercent { get; set; }
        //欠料
        public decimal OnNoQty { get; set; }
        public decimal OnNoPercent { get; set; }

        public string GetMaterials(int choose)
        {
            DataTable data = MaterialsModels.MaterialTable(choose);
            data.Columns["useDate"].ColumnName = "使用日期";
            data.Columns["csmItemCode"].ColumnName = "物料编码";
            data.Columns["useQty"].ColumnName = "使用数量";
            data.Columns["itemUnit"].ColumnName = "单位";
            data.Columns["itemDesp"].ColumnName = "物料描述";
            data.Columns["itemAttr1"].ColumnName = "备注";
            data.Columns["buyDate"].ColumnName = "物料需求日";
            if (choose == 0)
            {
                data.Columns["sysType"].ColumnName = "类型";
            }
            return PMPublicFuncs.DatatableToJson(data);      
        } 
    }
}
