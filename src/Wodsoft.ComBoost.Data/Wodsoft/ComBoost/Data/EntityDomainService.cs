using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Wodsoft.ComBoost.Data
{
    public class EntityDomainService<T> : DomainService
        where T : class, new()
    {
        public virtual Task Create([FromService] IDatabaseContext database)
        {
            var context = database.GetContext<T>();
            EntityEditModel<T> model = new EntityEditModel<T>(context.Create());
            ExecutionContext.DomainContext.Result = model;
            return Task.FromResult(0);
        }

        public virtual async Task Update([FromService] IDatabaseContext database, [FromService] IValueProvider valueProvider)
        {
            var context = database.GetContext<T>();

            ExecutionContext.DomainContext.Result = await database.SaveAsync();
        }

        protected virtual Task UpdateCore(IDomainContext serviceContext, T entity)
        {
            return null;
        }

        protected virtual Task UpdateProperty(IDomainContext serviceContext, T entity)
        {
            return null;

        }

        public virtual Task Remove(IDomainContext serviceContext)
        {
            return null;

        }

        public virtual Task Detail(IDomainContext serviceContext)
        {
            return null;
        }
    }
}
