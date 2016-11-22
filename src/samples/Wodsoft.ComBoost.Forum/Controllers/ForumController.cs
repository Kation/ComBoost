using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wodsoft.ComBoost.Mvc;
using Wodsoft.ComBoost.Forum.Entity;
using Wodsoft.ComBoost.Data;
using System.ComponentModel;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wodsoft.ComBoost.Forum.Controllers
{
    public class ForumController : DomainController
    {
        public async Task<IActionResult> Index()
        {
            var context = CreateDomainContext();
            var domain = DomainProvider.GetService<EntityDomainService<Thread>>();
            var result = await domain.ExecuteAsync<IEntityViewModel<Thread>>(context, "List");
            return View(result);
        }

    }
}
