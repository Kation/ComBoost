using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace System.Security
{
    public sealed class MemberManager
    {
        private static MemberManagerProvider provider;
        public static bool IsRunService { get; set; }

        static MemberManager()
        {
            Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings.AllKeys.Contains("MemberManagerProvider"))
            {
                try
                {
                    Type type = System.Reflection.TypeDelegator.GetType(config.AppSettings.Settings["MemberManagerProvider"].Value);
                    if (type!=null)
                    {
                        provider = (MemberManagerProvider)Activator.CreateInstance(type);
                        return;
                    }
                }
                catch
                {

                }                
            }
            provider = new DefaultMemberManager();
        }

        public static bool IsEnabled { get { return provider != null; } }

        public static bool ChangePassword(string username, string newPassword)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("成员管理器没有开启。");
            return provider.ChangePassword(username, newPassword);
        }

        public static bool Delete(string username)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("成员管理器没有开启。");
            return provider.Delete(username);
        }

        public static bool Create(string username, string email, string password)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("成员管理器没有开启。");
            return provider.Create(username, email, password);
        }

        public static bool ContainsUsername(string username)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("成员管理器没有开启。");
            return provider.ContainsUsername(username);
        }

        public static bool IsSigned
        {
            get
            {
                if (IsRunService)
                    return Wodsoft.Net.Service.ServiceContext.Current.Session["ComBoost_CurrentMember"] != null;
                else
                    return _MemberInfo != null;
            }
        }

        private static MemberInfo _MemberInfo;
        public static MemberInfo GetMemberInfo()
        {
            if (!IsEnabled)
                throw new InvalidOperationException("成员管理器没有开启。");
            return (MemberInfo)Wodsoft.Net.Service.ServiceContext.Current.Session["ComBoost_CurrentMember"];
        }

        public static bool Validate(string username, string password)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("成员管理器没有开启。");
            return provider.Verify(username, password);
        }

        public static bool SignIn(string username, string password)
        {
            if (!IsEnabled)
                throw new InvalidOperationException("成员管理器没有开启。");
            if (IsSigned)
                return false;
            if (!provider.Verify(username, password))
                return false;
            if (IsRunService)
                Wodsoft.Net.Service.ServiceContext.Current.Session["ComBoost_CurrentMember"] = provider.GetMemberInfo(username);
            else
                _MemberInfo = provider.GetMemberInfo(username);
            return true;
        }
        
        public static bool SignOut()
        {
            if (!IsEnabled)
                throw new InvalidOperationException("成员管理器没有开启。");
            if (!IsSigned)
                return false;
            if (IsRunService)
                Wodsoft.Net.Service.ServiceContext.Current.Session["ComBoost_CurrentMember"] = null;
            else
                _MemberInfo = null;
            return true;
        }
    }
}
