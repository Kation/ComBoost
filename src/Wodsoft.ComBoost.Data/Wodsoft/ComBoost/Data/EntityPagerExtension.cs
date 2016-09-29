using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace Wodsoft.ComBoost.Data
{
    public static class EntityPagerExtension
    {
        static EntityPagerExtension()
        {
            DefaultPageSize = 20;
            PageSizeOptions = new int[] { 10, 20, 30, 50 };
        }

        public static int DefaultPageSize { get; set; }

        public static int[] PageSizeOptions { get; set; }
    }

    public class EntityPagerExtension<T> : DomainExtension
        where T : class, IEntity, new()
    {
        private EntityDomainService<T> Service;

        public override void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService)
        {
            Service = (EntityDomainService<T>)domainService;
            Service.EntityQuery += Service_EntityQuery;
        }

        private void Service_EntityQuery(IDomainExecutionContext context, EntityQueryEventArgs<T> e)
        {
            var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
            int page = valueProvider.GetValue<int>("page");
            if (page == 0)
                page = 1;
            int size = valueProvider.GetValue<int>("size");
            if (size == 0)
                size = EntityPagerExtension.DefaultPageSize;
            e.Queryable = e.Queryable.Skip((page - 1) * size).Take(size);
        }
    }
}
