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
    public class PostDomainExtension<T> : DomainExtension
        where T : class, IPost, new()
    {
        private EntityDomainService<T> _Domain;

        public override void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService)
        {
            _Domain = (EntityDomainService<T>)domainService;
            _Domain.EntityQuery += Domain_EntityQuery;
            _Domain.EntityPreUpdate += Domain_EntityPreUpdate;
        }

        private async Task Domain_EntityPreUpdate(IDomainExecutionContext context, EntityUpdateEventArgs<T> e)
        {
            if (e.Entity.Member == null)
            {
                var authProvider = context.DomainContext.GetRequiredService<IAuthenticationProvider>();
                e.Entity.Member = await authProvider.GetAuthentication().GetPermission<IMember>();
            }
        }

        private Task Domain_EntityQuery(IDomainExecutionContext context, EntityQueryEventArgs<T> e)
        {
            var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
            var threadId = valueProvider.GetValue<Guid?>("id");
            if (!threadId.HasValue)
                return Task.CompletedTask;
            Guid key = threadId.Value;
            e.Queryable = e.Queryable.Wrap<IPost, T>().Where(t => t.Thread.Index == threadId.Value.Wrap()).Unwrap<IPost, T>();
            return Task.CompletedTask;
        }
    }
}
