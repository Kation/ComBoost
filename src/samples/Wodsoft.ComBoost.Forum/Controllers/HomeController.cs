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

namespace Wodsoft.ComBoost.Forum.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var user = User;
            bool isAuth = user.Identity.IsAuthenticated;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Test()
        {
            var databaseContext = HttpContext.RequestServices.GetRequiredService<IDatabaseContext>();
            var forumContext = databaseContext.GetContext<Entity.Forum>();
            var threadContext = databaseContext.GetContext<Entity.Thread>();
            //var thread = await threadContext.Query().FirstAsync();
            //var forum = await thread.LazyLoadEntityAsync(t => t.Forum, forumContext);
            //var forum = await forumContext.Query().FirstAsync();
            //var threadQuery = forum.LazyLoadQuery(t => t.Threads, threadContext);
            //var result = await threadQuery.ToArrayAsync();
            //var forum = new Entity.Forum();
            //forum.Index = Guid.NewGuid();
            //forum.CreateDate = forum.EditDate = DateTime.Now;
            //forum.Name = "测试板块";
            //context.Forum.Add(forum);
            //var thread = new Entity.Thread();
            //thread.Index = Guid.NewGuid();
            //thread.Forum = forum;
            //thread.Member = await context.Member.SingleAsync(t => t.Username.ToLower() == "admin");
            //thread.CreateDate = thread.EditDate = DateTime.Now;
            //thread.Title = "测试主题";
            //context.Thread.Add(thread);
            //await context.SaveChangesAsync();
            return Content("Success");
        }
    }
}
