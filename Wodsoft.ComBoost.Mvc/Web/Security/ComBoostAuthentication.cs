using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Routing;

namespace System.Web.Security
{
    public sealed class ComBoostAuthentication
    {
        private static System.Configuration.Configuration _Config;
        private static byte[] _Key;

        static ComBoostAuthentication()
        {
            _Config = WebConfigurationManager.OpenWebConfiguration("~");
            SystemWebSectionGroup system = (SystemWebSectionGroup)_Config.GetSectionGroup("system.web");
            IsEnabled = system.Authentication.Mode == AuthenticationMode.Forms;
            if (!IsEnabled)
                return;
            if (!_Config.AppSettings.Settings.AllKeys.Contains("ComBoostAuthenticationKey"))
            {
                _Key = Guid.NewGuid().ToByteArray();
                _Config.AppSettings.Settings.Add("ComBoostAuthenticationKey", Convert.ToBase64String(_Key));
                _Config.Save();
            }
            else
            {
                _Key = Convert.FromBase64String(_Config.AppSettings.Settings["ComBoostAuthenticationKey"].Value);
            }
            CookieDomain = system.Authentication.Forms.Domain;
            CookieName = "comboostauth";
            CookieName = system.Authentication.Forms.Name;
            CookiePath = system.Authentication.Forms.Path;
            Timeout = system.Authentication.Forms.Timeout;
        }

        public static string CookieDomain { get; private set; }

        public static string CookieName { get; private set; }

        public static string CookiePath { get; private set; }

        public static bool IsEnabled { get; private set; }

        public static TimeSpan Timeout { get; private set; }

        public static void RefreshSecurityKey()
        {
            _Key = Guid.NewGuid().ToByteArray();
            _Config.AppSettings.Settings.Add("ComBoostAuthenticationKey", Convert.ToBase64String(_Key));
            _Config.Save();
        }

        public static string CreateCookies(string username, string authArea)
        {
            return CreateCookies(username, authArea, Timeout);
        }

        public static string CreateCookies(string username, string authArea, TimeSpan timeout)
        {
            ComBoostCookiesToken token = new ComBoostCookiesToken();
            token.Username = username;
            token.ExpiredDate = DateTime.Now.Add(timeout);
            byte[] data;
            if (authArea == null)
                data = Encoding.UTF8.GetBytes(token.Username).Concat(BitConverter.GetBytes(token.ExpiredDate.ToBinary())).Concat(_Key).ToArray();
            else
                data = Encoding.UTF8.GetBytes(token.Username).Concat(BitConverter.GetBytes(token.ExpiredDate.ToBinary())).Concat(Encoding.UTF8.GetBytes(authArea)).Concat(_Key).ToArray();
            using (SHA1 sha1 = SHA1.Create())
                token.Signature = sha1.ComputeHash(data);
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, token);
            data = stream.ToArray();
            stream.Dispose();
            return HttpServerUtility.UrlTokenEncode(data);

        }

        public static bool VerifyCookie(string cookieValue, string authArea, out string username)
        {
            username = null;
            byte[] data;
            try
            {
                data = HttpServerUtility.UrlTokenDecode(cookieValue);
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(data);
                ComBoostCookiesToken token = (ComBoostCookiesToken)formatter.Deserialize(stream);
                stream.Dispose();

                if (token.Signature.Length != 20)
                    return false;
                if (token.ExpiredDate < DateTime.Now)
                    return false;
                if (token.Username == null)
                    return false;
                if (authArea == null)
                    data = Encoding.UTF8.GetBytes(token.Username).Concat(BitConverter.GetBytes(token.ExpiredDate.ToBinary())).Concat(_Key).ToArray();
                else
                    data = Encoding.UTF8.GetBytes(token.Username).Concat(BitConverter.GetBytes(token.ExpiredDate.ToBinary())).Concat(Encoding.UTF8.GetBytes(authArea)).Concat(_Key).ToArray();

                using (SHA1 sha1 = SHA1.Create())
                    data = sha1.ComputeHash(data);
                for (int i = 0; i < 20; i++)
                    if (data[i] != token.Signature[i])
                        return false;
                username = token.Username;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SignIn(string username, bool forever)
        {
            if (forever)
                SignIn(username, TimeSpan.FromDays(360));
            else
                SignIn(username, Timeout);
        }

        public static void SignIn(string username, TimeSpan timeout)
        {
            RouteData route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            string cookieValue;
            string authArea = null;
            if (route.DataTokens.ContainsKey("authArea"))
                authArea = route.DataTokens["authArea"].ToString();
            cookieValue = CreateCookies(username, authArea, timeout);
            string cookieName;
            if (authArea == null)
                cookieName = CookieName;
            else
                cookieName = CookieName + "_" + authArea;
            HttpCookie cookie;
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains(cookieName))
                cookie = HttpContext.Current.Response.Cookies[cookieName];
            else
                cookie = new HttpCookie(cookieName);
            cookie.Value = cookieValue;
            cookie.Domain = CookieDomain;
            cookie.Expires = DateTime.Now.Add(timeout);
            cookie.HttpOnly = true;
            cookie.Path = CookiePath;
            HttpContext.Current.Response.Cookies.Remove(cookieName);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void SignOut()
        {
            RouteData route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            string authArea = null;
            if (route.DataTokens.ContainsKey("authArea"))
                authArea = route.DataTokens["authArea"].ToString();
            if (authArea == null)
            {
                if (HttpContext.Current.Response.Cookies.AllKeys.Contains(CookieName))
                    HttpContext.Current.Response.Cookies.Remove(CookieName);
            }
            else
            {
                if (HttpContext.Current.Response.Cookies.AllKeys.Contains(CookieName + "_" + authArea))
                    HttpContext.Current.Response.Cookies.Remove(CookieName + "_" + authArea);
            }
        }
    }
}
