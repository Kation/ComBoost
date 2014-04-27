using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public class EntityAuthorizeAttribute : AuthorizeAttribute
    {
        protected IEntityContextBuilder EntityBuilder { get; private set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;
            EntityBuilder = httpContext.Items["EntityBuilder"] as IEntityContextBuilder;
            return true;
        }
    }
}
