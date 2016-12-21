using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.Mvc;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wodsoft.ComBoost.Forum.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ForumController : EntityController<Entity.Forum>
    {
        public async Task<IActionResult> ImageToProperty(string path)
        {
            var storage = HttpContext.RequestServices.GetRequiredService<IStorageProvider>().GetStorage();
            var file = await storage.GetAsync(path);
            if (file == null)
                return NotFound();
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            string mimeType;
            if (!provider.TryGetContentType(path, out mimeType))
                mimeType = "application/octet-stream";
            return File(file, mimeType);
        }
    }
}
