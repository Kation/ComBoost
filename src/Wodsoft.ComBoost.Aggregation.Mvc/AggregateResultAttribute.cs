using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Aggregation.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AggregateResultAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedResult = await next();
            if (executedResult.Result is JsonResult jsonResult)
            {
                if (jsonResult.Value != null)
                {
                    var aggregator = context.HttpContext.RequestServices.GetService<IDomainAggregator>();
                    jsonResult.Value = await AggregateAsync(aggregator, jsonResult.Value);
                }
            }
            else if (executedResult.Result is ObjectResult objectResult)
            {
                if (objectResult.Value != null)
                {
                    var aggregator = context.HttpContext.RequestServices.GetService<IDomainAggregator>();
                    objectResult.Value = await AggregateAsync(aggregator, objectResult.Value);
                }
            }
        }

        private async Task<object> AggregateAsync(IDomainAggregator aggregator, object value)
        {
            var type = value.GetType();
            if (value is Array arrayValue)
            {
                var elementType = type.GetElementType();
                var builderType = typeof(DomainAggregationsBuilder<>).MakeGenericType(elementType);
                var aggregationType = (Type)builderType.GetProperty(nameof(DomainAggregationsBuilder<object>.AggregationType)).GetValue(null);
                if (aggregationType == null)
                    return value;
                var length = arrayValue.Length;
                var array = Array.CreateInstance(aggregationType, length);
                Task[] tasks = new Task[length];
                for (int i = 0; i < length; i++)
                {
                    var index = i;
                    var item = arrayValue.GetValue(i);
                    tasks[index] = aggregator.AggregateAsync(item, elementType).ContinueWith(task =>
                    {
                        array.SetValue(task.Result, index);
                    });
                }
                await Task.WhenAll(tasks);
                return array;
            }
            var readOnlyListType = type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IReadOnlyList<>));
            if (readOnlyListType != null)
            {
                var elementType = readOnlyListType.GetGenericArguments()[0];
                var builderType = typeof(DomainAggregationsBuilder<>).MakeGenericType(elementType);
                var aggregationType = (Type)builderType.GetProperty(nameof(DomainAggregationsBuilder<object>.AggregationType)).GetValue(null);
                if (aggregationType == null)
                    return value;
                var length = (int)readOnlyListType.GetProperty("Count").GetValue(value);
                var array = Array.CreateInstance(aggregationType, length);
                Task[] tasks = new Task[length];
                int i = 0;
                foreach(var item in (IEnumerable)value)
                {
                    var index = i;
                    tasks[index] = aggregator.AggregateAsync(item, elementType).ContinueWith(task =>
                    {
                        array.SetValue(task.Result, index);
                    });
                    i++;
                }
                return array;
            }
            else
            {
                return await aggregator.AggregateAsync(value);
            }
        }
    }
}
