using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wodsoft.ComBoost.Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Init()
        {
            string[] langs = Request.UserLanguages;
            string lang;
            if (langs.Length == 0)
                lang = "en-us";
            else
                lang = langs[0].ToLower();
            if (lang == "zh-hans-cn")
                lang = "zh-cn";
            return RedirectToAction("Index", new { lang = lang });
        }
    }
}