using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMSettings;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;

namespace PlanMateWebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMvcCore()
                .AddNewtonsoftJson(options => {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            // RegisterMyServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Index}/{action=Index}/{id?}");
            });
        }

        //Set System Configrations
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            string filepach = AppContext.BaseDirectory;
            PMAppSettings.BasePath = AppContext.BaseDirectory;
            XmlDocument document = new XmlDocument();
            document.Load(filepach + "appsettings.xml");
            XmlNode TableFileConfig = document.SelectSingleNode("AppSetting").SelectSingleNode("TableFileds");
            XmlNode ConnectionConfig = document.SelectSingleNode("AppSetting").SelectSingleNode("ConnectionStrings");
            XmlNode PMSettingsConfig = document.SelectSingleNode("AppSetting").SelectSingleNode("PMSettings");
            XmlNodeList ConnectionList = ConnectionConfig.ChildNodes;
            XmlNodeList TableFiledsConfigList = TableFileConfig.ChildNodes;
            XmlNodeList PMSettingConfigList = PMSettingsConfig.ChildNodes;
            PMAppSettings.TableFileds = new JObject();
            JObject temp;
            foreach (XmlNode item in TableFiledsConfigList)
            {
                XmlNodeList xl = item.ChildNodes;
                temp = new JObject();
                foreach (XmlNode filed in xl)
                {
                   temp.Add(filed.Name, filed.InnerText);
                }
                PMAppSettings.TableFileds.Add(item.Name.ToString(), temp);
            }

            foreach (XmlNode item in ConnectionList)
            {
                if (item.Name.ToLower() == "mod")
                {
                    PMAppSettings.Modconnstr = item.InnerText;
                }
                else if (item.Name.ToLower() == "sch")
                {
                    PMAppSettings.Schconnstr = item.InnerText;
                }
                else
                {
                    PMAppSettings.Ctrlconnstr = item.InnerText;
                }
            }
            foreach (XmlNode item in PMSettingConfigList)
            {
                if (item.Name.ToLower() == "plstate")
                {
                    PMAppSettings.PMPlState = item.InnerText;
                }
                else if (item.Name.ToLower() == "ocstate")
                {
                    PMAppSettings.PMOcState = item.InnerText;
                }
            }

            //Configuration = configuration;
            //var settings = Configuration.GetSection("PMSettings").GetChildren();
            //if (PMAppSettings.PMSettings.Columns.Count< 1)
            //{
            //    PMAppSettings.PMSettings.Columns.Add("KEY");
            //    PMAppSettings.PMSettings.Columns.Add("VALUE");
            //    PMAppSettings.PMSettings.Columns.Add("PACH");
            //}
            //foreach (var item in settings)
            //{
            //    string pmkey = item.Key.ToString();
            //    string pmvalue = item.Value.ToString();
            //    string pmsettingspach = item.Path.ToString();
            //    DataRow dr = PMAppSettings.PMSettings.NewRow();
            //    dr[0] = pmkey;
            //    dr[1] = pmvalue;
            //    dr[2] = pmsettingspach;
            //    PMAppSettings.PMSettings.Rows.Add(dr);                     
            //    if(item.Key.ToString().ToLower().Contains("plstate"))
            //    {
            //        PMAppSettings.PMPlState = item.Value.ToString();
            //    }
            //    else if (item.Key.ToString().ToLower().Contains("ocstate"))
            //    {
            //        PMAppSettings.PMOcState = item.Value.ToString();
            //    }
               
            //} 
        }
    }
}
