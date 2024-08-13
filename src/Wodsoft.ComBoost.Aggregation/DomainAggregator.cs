using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation
{
    public class DomainAggregator : IDomainAggregator
    {
        private IServiceProvider _services;

        public DomainAggregator(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public Task<T> AggregateAsync<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!DomainAggregationsBuilder<T>.HasAggregation)
                return Task.FromResult(value);
            IDomainAggregation aggregation = (IDomainAggregation)DomainAggregationsBuilder<T>.Constructor!.Invoke(new object[] { value });
            return aggregation.AggregateAsync(this).ContinueWith(task =>
            {
                if (task.IsFaulted)
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(task.Exception).Throw();
                return (T)aggregation;
            });
        }

        public Task<object> AggregateAsync(object value, Type valueType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            var builderType = typeof(DomainAggregationsBuilder<>).MakeGenericType(valueType);
            if (!(bool)builderType.GetProperty("HasAggregation", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null))
                return Task.FromResult(value);
            IDomainAggregation aggregation = (IDomainAggregation)((ConstructorInfo)builderType.GetProperty("Constructor", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).Invoke(new object[] { value });
            return aggregation.AggregateAsync(this).ContinueWith(task => (object)aggregation);
        }

        public async Task<T?> GetAggregationAsync<T>(object[] keys)
        {
            var providers = _services.GetServices<IDomainAggregatorProvider<T>>().ToArray();
            if (providers.Length == 0)
                return default;
            DomainAggregatorExecutionPipeline<T>? pipeline = null;
            for (int i = providers.Length - 1; i >= 0; i--)
            {
                var next = pipeline;
                var index = i;
                pipeline = () => providers[index].GetAsync(keys, next);
            }
            return await pipeline!();
        }
    }
}
