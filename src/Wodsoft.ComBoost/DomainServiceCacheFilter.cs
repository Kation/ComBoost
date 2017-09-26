using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainServiceCacheFilter : DomainServiceFilterAttribute
    {
        public DomainServiceCacheFilter(Type valueType, TimeSpan? expireTime)
            : this(valueType, expireTime, new string[0])
        { }

        public DomainServiceCacheFilter(Type valueType, TimeSpan? expireTime, params string[] parameters)
        {
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            foreach (var parameter in parameters)
                if (string.IsNullOrEmpty(parameter))
                    throw new ArgumentNullException(nameof(parameters), "参数名有空值。");
            ValueType = valueType;
            ExpireTime = expireTime;
            Parameters = parameters;
        }

        public Type ValueType { get; private set; }

        public TimeSpan? ExpireTime { get; private set; }

        public string[] Parameters { get; private set; }

        public override async Task OnExecutingAsync(IDomainExecutionContext context)
        {
            var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
            var key = GetCacheKey(context, valueProvider);
            var cacheProvider = context.DomainContext.GetRequiredService<ICacheProvider>();
            var value = await cacheProvider.GetCache().GetAsync(key, ValueType);
            if (value != null)
                context.Done(value);
        }

        public override Task OnExecutedAsync(IDomainExecutionContext context)
        {
            var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
            var key = GetCacheKey(context, valueProvider);
            var cacheProvider = context.DomainContext.GetRequiredService<ICacheProvider>();
            return cacheProvider.GetCache().SetAsync(key, context.Result, ExpireTime);
        }

        protected virtual string GetCacheKey(IDomainExecutionContext context, IValueProvider valueProvider)
        {
            string key = "__ComBoostCache_" + context.DomainService.GetType().Name + "_" + context.DomainMethod.Name;
            foreach (var parameter in Parameters)
            {
                var value = valueProvider.GetValue<string>(parameter);
                if (value == null)
                {
                    key += "_";
                    continue;
                }
                var hash = Convert.ToBase64String(BitConverter.GetBytes(value.GetHashCode()));
                key += "_" + hash;
            }
            return key;
        }
    }
}
