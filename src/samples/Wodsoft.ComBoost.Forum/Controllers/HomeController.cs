using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Mvc;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Forum.Entity;
using System.ComponentModel;

namespace Wodsoft.ComBoost.Forum.Controllers
{
    public class HomeController : DomainController
    {
        public async Task<IActionResult> Index()
        {
            var context = CreateDomainContext();
            context.ValueProvider.SetValue("size", 1000);
            var domain = DomainProvider.GetService<EntityDomainService<Entity.Forum>>();
            var result = await domain.ExecuteAsync<IEntityViewModel<Entity.Forum>>(context, "List");
            return View(result);
        }
    }
}
