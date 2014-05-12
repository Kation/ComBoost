using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    public class ComBoostAuthenticationModel : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.AuthorizeRequest += context_AuthorizeRequest;
        }

        private void context_AuthorizeRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
                HttpContext.Current.User = new ComBoostPrincipal(HttpContext.Current.User);
        }
    }
}
