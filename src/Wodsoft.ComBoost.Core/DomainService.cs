using System;
using System.Collections.Generic;
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
        }

        #endregion
    }
}
