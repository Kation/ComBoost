using System.Web;
using System.Web.Mvc;

namespace Wodsoft.ComBoost.Website
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}