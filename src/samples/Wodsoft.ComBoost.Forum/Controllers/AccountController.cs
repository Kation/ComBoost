using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wodsoft.ComBoost.Mvc;
using Wodsoft.ComBoost.Forum.Domain;
using Wodsoft.ComBoost.Security;
using Wodsoft.ComBoost.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wodsoft.ComBoost.Forum.Controllers
{
    public class AccountController : DomainController
    {
        public IActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string username)
        {
            if (User.Identity.IsAuthenticated)
                return StatusCode(200);
            var memberDomain = DomainProvider.GetService<MemberDomain>();
            var context = CreateDomainContext();
            try
            {
                await memberDomain.ExecuteAsync<IAuthenticationProvider, string, string>(context, memberDomain.SignIn);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
            return StatusCode(200);
        }

        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string username)
        {
            if (User.Identity.IsAuthenticated)
                return StatusCode(200);
            var memberDomain = DomainProvider.GetService<MemberDomain>();
            var context = CreateDomainContext();
            //try
            //{
            await memberDomain.ExecuteAsync<IAuthenticationProvider, IDatabaseContext, string, string>(context, memberDomain.SignUp);
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(400, ex.Message);
            //}
            return StatusCode(200);
        }
    }
}
