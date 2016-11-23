using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Forum.Core;
using Microsoft.Extensions.DependencyInjection;

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
        }

        private void Domain_EntityQuery(IDomainExecutionContext context, EntityQueryEventArgs<T> e)
        {
            var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
            var forumId = valueProvider.GetValue<Guid?>("id");
            if (!forumId.HasValue)
                return;
            object key = forumId.Value;
            e.Queryable = e.Queryable.Where(t => t.Forum.Index == key);
        }
    }
}
