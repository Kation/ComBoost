using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wodsoft.ComBoost.Mvc;
using Wodsoft.ComBoost.Forum.Domain;
using Wodsoft.ComBoost.Security;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wodsoft.ComBoost.Forum.Controllers
{
    public class ThreadController : DomainController
    {
        public async Task<IActionResult> Create()
        {
            var threadDomain = DomainProvider.GetService<ThreadDomain>();
            var context = CreateDomainContext();
            try
            {
                var thread = await threadDomain.ExecuteAsync<IAuthenticationProvider, IDatabaseContext, IForum, string, string, IThread>(context, threadDomain.Create);
                return View(thread);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
