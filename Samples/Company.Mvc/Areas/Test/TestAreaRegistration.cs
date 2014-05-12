using Company.Entity;
using System.Web.Mvc;

namespace Company.Mvc.Areas.Test
{
    public class TestAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Test";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.MapRoute<Employee>(
                "Test_default",
                "Test/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional},
                new string[] { "Company.Mvc.Areas.Test.Controllers" }
            );
        }
    }
}