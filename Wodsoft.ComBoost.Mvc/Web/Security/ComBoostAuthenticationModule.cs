using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    /// <summary>
    /// ComBoost authentication http module.
    /// </summary>
    public class ComBoostAuthenticationModule : IHttpModule
    {
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
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
