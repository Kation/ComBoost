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

        private string[] _Supported = new string[]
        {
            "en-us",
            "zh-cn"
        };
        public ActionResult Init()
        {
            string[] langs = Request.UserLanguages;
            string lang;
            if (langs == null || langs.Length == 0)
                lang = "en-us";
            else
                lang = langs[0].ToLower();
            if (lang == "zh-hans-cn")
                lang = "zh-cn";
            if (!_Supported.Contains(lang))
                lang = "en-us";
            return RedirectToAction("Index", new { lang = lang });
        }

        public ActionResult License()
        {
            return View();
        }
    }
}