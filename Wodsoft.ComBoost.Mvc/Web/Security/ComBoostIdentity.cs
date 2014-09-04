using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    public class ComBoostIdentity : IIdentity
    {
        internal ComBoostIdentity(ComBoostPrincipal principal)
        {
            _Principal = principal;
        }

        private ComBoostPrincipal _Principal;

        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        private bool? _IsAuthenticated;
        public bool IsAuthenticated
        {
            get
            {
                if (_IsAuthenticated == null)
                {
                    HttpContext context = HttpContext.Current;
                    string name;
                    string authArea = null;
                    if (_Principal.CurrentRoute == null || !_Principal.CurrentRoute.DataTokens.ContainsKey("authArea"))
                        name = ComBoostAuthentication.CookieName;
                    else
                    {
                        authArea = _Principal.CurrentRoute.DataTokens["authArea"].ToString();
                        name = ComBoostAuthentication.CookieName + "_" + authArea;
                    }

                    object state = context.Items[name];
                    if (state == null)
                    {
                        if (!context.Request.Cookies.AllKeys.Contains(name))
                        {
                            _IsAuthenticated = false;
                        }
                        else
                        {
                            string cookies = context.Request.Cookies[name].Value;
                            _IsAuthenticated = ComBoostAuthentication.VerifyCookie(cookies, authArea, out _Name);
                            if (_IsAuthenticated.Value && _Principal.RoleEntity != null)
                                ComBoostAuthentication.SignIn(_Name, false);
                        }
                    }
                    else
                    {
                        _IsAuthenticated = (bool)state;
                    }
                }
                return _IsAuthenticated.Value;
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                if (!IsAuthenticated)
                    return null;
                return _Name;
            }
        }
    }
}
