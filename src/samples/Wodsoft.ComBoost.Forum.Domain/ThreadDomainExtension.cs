using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public class ThreadDomainExtension<T> : DomainExtension
        where T : class, IThread, new()
    {
        private EntityDomainService<T> _Domain;

        public override void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService)
        {
            _Domain = (EntityDomainService<T>)domainService;
            _Domain.EntityQuery += Domain_EntityQuery;
            _Domain.EntityPreUpdate += Domain_EntityPreUpdate;
            //_Domain.EntityPropertyUpdate += _Domain_EntityPropertyUpdate;
        }

        private void _Domain_EntityPropertyUpdate(IDomainExecutionContext context, EntityPropertyUpdateEventArgs<T> e)
        {
            if (e.Property.ClrName == "Member")
            {
                if (e.Entity.Member == null)
                {
                    var provider = context.DomainContext.GetRequiredService<IAuthenticationProvider>();
                    e.Entity.Member = provider.GetAuthentication().GetPermission<IMember>().Result;
                }
                e.IsHandled = true;
            }            
        }

        private void Domain_EntityPreUpdate(IDomainExecutionContext context, EntityUpdateEventArgs<T> e)
        {
            //var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
            //var forumId = valueProvider.GetRequiredValue<Guid>("forum");
            if (e.Entity.Member == null)
            {
                var authProvider = context.DomainContext.GetRequiredService<IAuthenticationProvider>();
                e.Entity.Member = authProvider.GetAuthentication().GetPermission<IMember>().Result;
            }
        }

        private void Domain_EntityQuery(IDomainExecutionContext context, EntityQueryEventArgs<T> e)
        {
            var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
            var forumId = valueProvider.GetValue<Guid?>("id");
            if (!forumId.HasValue)
                return;
            Guid key = forumId.Value;
            e.Queryable = e.Queryable.Wrap<IThread, T>().Where(t => t.Forum.Index == forumId.Value.Wrap()).Unwrap<IThread, T>();
        }
    }
}
