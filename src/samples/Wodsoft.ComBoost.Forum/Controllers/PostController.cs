using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wodsoft.ComBoost.Mvc;
using Wodsoft.ComBoost.Forum.Domain;
using Wodsoft.ComBoost.Forum.Entity;
using Wodsoft.ComBoost.Data;
using System.ComponentModel;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wodsoft.ComBoost.Forum.Controllers
{
    public class PostController : DomainController
    {
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var context = CreateDomainContext();
            var domain = DomainProvider.GetService<EntityDomainService<Post>>();
            try
            {
                var model = await domain.ExecuteAsync<IEntityUpdateModel<Post>>(context, "Update");
                return RedirectToAction("Index", "Thread", new { id = model.Result.Thread.Index });
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
