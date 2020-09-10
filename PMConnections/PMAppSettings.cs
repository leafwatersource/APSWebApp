using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PMSettings
{
    public static class PMAppSettings
    {
        public static string Modconnstr { get; set; }
        public static string Schconnstr { get; set; }
        public static string Ctrlconnstr { get; set; }
        public static string PMPlState { get; set; }
        public static string PMOcState { get; set; }
        public static DataTable PMSettings = new DataTable();
        public static string ItemAttrCount { get; set; }
        public static DataTable ItemGroup = new DataTable();
        public static JObject TableFileds { get; set; }
        public static List<string> LiErrorMsg = new List<string>();
        public static string BasePath { get; set; }
    }
}
