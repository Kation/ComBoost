using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainService : IDomainService
    {
        public IDomainExecutionContext Context { get; private set; }

        #region 引发事件

        protected virtual Task RaiseEvent<TArgs>(TArgs e)
            where TArgs : DomainServiceEventArgs
        {
            return Context.DomainContext.EventManager.RaiseEvent(Context, e);
        }

        void IDomainService.Initialize(IDomainExecutionContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {

        }

        #endregion

        public static string GetServiceName(Type type)
        {
            string name = type.Name;
            if (type.IsGenericType)
                name = name.Split('`')[0];
            if (name.EndsWith("DomainService"))
                name = name.Substring(0, name.Length - "DomainService".Length);
            if (type.IsGenericType)
            {
                name += "(" + string.Join(",", type.GetGenericArguments().Select(t => GetServiceName(t))) + ")";
            }
            return name;
        }
    }
}
