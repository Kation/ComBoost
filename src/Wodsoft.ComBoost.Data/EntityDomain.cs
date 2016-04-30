using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace Wodsoft.ComBoost.Data
{
    public class EntityDomain<T>
    {
        public virtual Task Create(IDomainContext serviceContext)
        {
            var database = serviceContext.GetService<IDatabaseContext>();
            var context = database.GetContext<T>();
            serviceContext.Result = context.Create();
            return Task.FromResult(0);
        }

        public virtual async Task Update(IDomainContext serviceContext)
        {
            IValueProvider valueProvider = serviceContext.GetService<IValueProvider>();
            if (valueProvider == null)
                throw new NotSupportedException("Could not get value provider from service context.");

            var database = serviceContext.GetService<IDatabaseContext>();
            var context = database.GetContext<T>();

            serviceContext.Result = await database.SaveAsync();
        }

        protected virtual Task UpdateCore(IDomainContext serviceContext, T entity)
        {

        }

        protected virtual Task UpdateProperty(IDomainContext serviceContext, T entity)
        {

        }

        public virtual Task Remove(IDomainContext serviceContext)
        {

        }

        public virtual Task Detail(IDomainContext serviceContext)
        {

        }
    }
}
