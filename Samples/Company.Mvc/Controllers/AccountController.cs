using Company.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Company.Mvc.Controllers
{
    public class AccountController : EntityController
    {
        public AccountController(IEntityContextBuilder builder) : base(builder) { }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                Response.StatusCode = 400;
                return Content("用户名不能为空！");
            }
            if (string.IsNullOrEmpty(password))
            {
                Response.StatusCode = 400;
                return Content("密码不能为空！");
            }
            var context = EntityBuilder.GetContext<Employee>();
            Employee employee = context.Query().SingleOrDefault(c => c.Name == username);
            if (employee == null)
            {
                Response.StatusCode = 400;
                return Content("该员工不存在！");
            }
            if (!employee.VerifyPassword(password))
            {
                Response.StatusCode = 400;
                return Content("密码错误！");
            }
            FormsAuthentication.SetAuthCookie(employee.Index.ToString(), false);
            return new HttpStatusCodeResult(200);
        }
    }
}