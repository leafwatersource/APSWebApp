using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PMSettings;
using System.Data;

namespace PlanMateWebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMvcCore().AddNewtonsoftJson(options => {
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
            Configuration = configuration;
            var settings = Configuration.GetSection("PMSettings").GetChildren();
            if(PMAppSettings.PMSettings.Columns.Count< 1)
            {
                PMAppSettings.PMSettings.Columns.Add("KEY");
                PMAppSettings.PMSettings.Columns.Add("VALUE");
                PMAppSettings.PMSettings.Columns.Add("PACH");
            }
            foreach (var item in settings)
            {
                string pmkey = item.Key.ToString();
                string pmvalue = item.Value.ToString();
                string pmsettingspach = item.Path.ToString();
                DataRow dr = PMAppSettings.PMSettings.NewRow();
                dr[0] = pmkey;
                dr[1] = pmvalue;
                dr[2] = pmsettingspach;
                PMAppSettings.PMSettings.Rows.Add(dr);                     
                if(item.Key.ToString().ToLower().Contains("plstate"))
                {
                    PMAppSettings.PMPlState = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower().Contains("ocstate"))
                {
                    PMAppSettings.PMOcState = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower().Contains("showattr"))
                {
                    PMAppSettings.ItemAttrCount = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "attr1")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr1");
                    DataRow row = PMAppSettings.ItemGroup.NewRow();
                    row[0] = item.Value.ToString();
                    PMAppSettings.ItemGroup.Rows.Add(row);
                    //AppSettings.ItemGroup.Rows[0]["ItemAttr1"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "attr2")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr2");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr2"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "attr3")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr3");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr3"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "attr4")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr4");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr4"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower()== "attr5")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr5");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr5"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "attr6")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr6");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr6"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "attr7")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr7");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr7"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() =="attr8")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr8");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr8"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "attr9")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr9");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr9"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "attr10")
                {
                    PMAppSettings.ItemGroup.Columns.Add("ItemAttr10");
                    PMAppSettings.ItemGroup.Rows[0]["ItemAttr10"] = item.Value.ToString();
                }
                else if (item.Key.ToString().ToLower() == "itemweight")
                {
                    PMAppSettings.ItemGroup.Columns.Add("itemWeight");
                    PMAppSettings.ItemGroup.Rows[0]["itemWeight"] = item.Value.ToString();
                }
            }
            PMAppSettings.Modconnstr = Configuration.GetConnectionString("Mod"); 
            PMAppSettings.Ctrlconnstr = Configuration.GetConnectionString("Ctrl");
            PMAppSettings.Schconnstr = Configuration.GetConnectionString("Sch");          
        }
    }
}
