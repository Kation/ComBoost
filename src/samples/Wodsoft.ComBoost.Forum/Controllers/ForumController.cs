using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wodsoft.ComBoost.Mvc;
using Wodsoft.ComBoost.Forum.Entity;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using System.ComponentModel;
using System.Runtime.ExceptionServices;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wodsoft.ComBoost.Forum.Controllers
{
    public class ForumController : DomainController
    {
        public async Task<IActionResult> Index()
        {
            var context = CreateDomainContext();
            var forumDomain = DomainProvider.GetService<EntityDomainService<Entity.Forum>>();
            IEntityEditModel<Entity.Forum> forumResult;
            try
            {
                forumResult = await forumDomain.ExecuteAsync<IEntityEditModel<Entity.Forum>>(context, "Detail");
                ViewBag.Forum = forumResult.Item;
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is ArgumentException)
                    return StatusCode(400, ex.InnerException.Message);
                else if (ex.InnerException is EntityNotFoundException)
                    return NotFound();
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            var threadDomain = DomainProvider.GetService<EntityDomainService<Thread>>();
            IEntityViewModel<Thread> threadResult = await threadDomain.ExecuteAsync<IEntityViewModel<Thread>>(context, "List");
            return View(threadResult);
        }

        public async Task<IActionResult> Create()
        {
            var context = CreateDomainContext();
            var forumDomain = DomainProvider.GetService<EntityDomainService<Entity.Forum>>();
            IEntityEditModel<Entity.Forum> forumResult;
            try
            {
                forumResult = await forumDomain.ExecuteAsync<IEntityEditModel<Entity.Forum>>(context, "Detail");
                ViewBag.Forum = forumResult.Item;
            }
            catch (DomainServiceException ex)
            {
                if (ex.InnerException is ArgumentException)
                    return StatusCode(400, ex.InnerException.Message);
                else if (ex.InnerException is EntityNotFoundException)
                    return NotFound();
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            var threadDomain = DomainProvider.GetService<EntityDomainService<Thread>>();
            IEntityEditModel<Thread> threadResult = await threadDomain.ExecuteAsync<IEntityEditModel<Thread>>(context, "Create");
            threadResult.Item.Forum = forumResult.Item;
            return View(threadResult);
        }
    }
}
