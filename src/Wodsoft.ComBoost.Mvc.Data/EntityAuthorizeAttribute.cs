using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityAuthorizeAttribute : ComBoostAuthorizeAttribute, IActionFilter
    {
        protected override bool AuthorizeCore(FilterContext context, object controller)
        {
            var entityMetadata = controller as IHaveEntityMetadata;
            if (controller == null)
                return true;
            return entityMetadata.Metadata.AllowAnonymous || context.HttpContext.User.Identity.IsAuthenticated;
        }        
    }    
}
