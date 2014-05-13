using Company.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Company.Mvc
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DataContext context = new DataContext();
            if (!context.Database.Exists())
            {
                context.Database.Create();

                EmployeeGroup group = context.EmployeeGroup.Create();
                group.Index = Guid.NewGuid();
                group.CreateDate = DateTime.Now;
                group.Name = "Admins";
                group.Power = EmployeePower.Admin;
                context.EmployeeGroup.Add(group);

                Employee admin = context.Employee.Create();
                admin.Index = Guid.NewGuid();
                admin.CreateDate = DateTime.Now;
                admin.Name = "admin";
                admin.Group = group;
                admin.SetPassword("admin");
                context.Employee.Add(admin);
                context.SaveChanges();
            }
            context.Dispose();
        }
    }
}