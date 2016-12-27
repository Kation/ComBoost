using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Wodsoft.ComBoost.Data
{
    public class EntitySearchExtension<T> : DomainExtension
        where T : class, IEntity, new()
    {
        private EntityDomainService<T> Service;

        public override void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService)
        {
            Service = (EntityDomainService<T>)domainService;
            Service.EntityQuery += Service_EntityQuery;
            Service.Executed += Service_Executed;
        }

        private Task Service_Executed(IDomainExecutionContext context)
        {
            if (context.DomainContext.DataBag.SearchItem != null && context.Result != null)
            {
                dynamic model = context.Result;
                model.SearchItem = context.DomainContext.DataBag.SearchItem;
            }
            return Task.FromResult(0);
        }

        private Task Service_EntityQuery(IDomainExecutionContext context, EntityQueryEventArgs<T> e)
        {
            List<EntitySearchItem> searchItems = new List<EntitySearchItem>();

            var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
            if (!valueProvider.GetValue<bool>("Search"))
#if NET451
            return Task.FromResult(0);
#else
                return Task.CompletedTask;
#endif

            IQueryable<T> queryable = e.Queryable;

            var keys = valueProvider.Keys.Where(t => t.StartsWith("Search.")).Select(t => t.Substring(7).Split('.')).GroupBy(t => t[0], t => t.Length == 1 ? "" : "." + t[1]).ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                string propertyName = keys[i].Key;
                IPropertyMetadata property = Service.Metadata.GetProperty(propertyName);
                if (property == null || !property.Searchable)
                    continue;
                EntitySearchItem searchItem = new EntitySearchItem();
                string[] options = keys[i].ToArray();
                switch (property.Type)
                {
                    case CustomDataType.Date:
                    case CustomDataType.DateTime:
                        for (int a = 0; a < options.Length; a++)
                        {
                            if (options[a] == ".Start")
                            {
                                DateTime start;
                                if (!DateTime.TryParse(valueProvider.GetValue<string>("Search." + keys[i].Key + options[a]), out start))
                                    continue;
                                searchItem.MorethanDate = start;
                                ParameterExpression parameter = Expression.Parameter(Service.Metadata.Type);
                                queryable = queryable.Where<T>(Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(start)), parameter));
                            }
                            else if (options[a] == ".End")
                            {
                                DateTime end;
                                if (!DateTime.TryParse(valueProvider.GetValue<string>("Search." + keys[i].Key + options[a]), out end))
                                    continue;
                                if (property.Type == CustomDataType.Date)
                                    end = end.AddDays(1);
                                searchItem.LessthanDate = end;
                                ParameterExpression parameter = Expression.Parameter(Service.Metadata.Type);
                                queryable = queryable.Where<T>(Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(end)), parameter));
                            }
                        }
                        break;
                    case CustomDataType.Boolean:
                    case CustomDataType.Sex:
                        if (options[0] == "")
                        {
                            bool result;
                            if (!bool.TryParse(valueProvider.GetValue<string>("Search." + keys[i].Key), out result))
                                continue;
                            searchItem.Equal = result;
                            ParameterExpression parameter = Expression.Parameter(Service.Metadata.Type);
                            queryable = queryable.Where<T>(Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Property(parameter, property.ClrName), Expression.Constant(result)), parameter));
                        }
                        break;
                    case CustomDataType.Currency:
                    case CustomDataType.Integer:
                    case CustomDataType.Number:
                        for (int a = 0; a < options.Length; a++)
                        {
                            if (options[a] == ".Start")
                            {
                                double start;
                                if (!double.TryParse(valueProvider.GetValue<string>("Search." + keys[i].Key + options[a]), out start))
                                    continue;
                                searchItem.Morethan = start;
                                ParameterExpression parameter = Expression.Parameter(Service.Metadata.Type);
                                queryable = queryable.Where<T>(Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(start)), parameter));
                            }
                            else if (options[a] == ".End")
                            {
                                double end;
                                if (!double.TryParse(valueProvider.GetValue<string>("Search." + keys[i].Key + options[a]), out end))
                                    continue;
                                searchItem.Lessthan = end;
                                ParameterExpression parameter = Expression.Parameter(Service.Metadata.Type);
                                queryable = queryable.Where<T>(Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(Expression.Property(parameter, property.ClrName), Expression.Constant(end)), parameter));
                            }
                        }
                        break;
                    case CustomDataType.Other:
                        if (property.CustomType == "Enum")
                        {
                            object result;
                            try
                            {
                                result = Enum.Parse(property.ClrType, valueProvider.GetValue<string>("Search." + keys[i].Key));
                            }
                            catch
                            {
                                continue;
                            }
                            searchItem.Enum = new EnumConverter(property.ClrType).ConvertToString(result);
                            ParameterExpression parameter = Expression.Parameter(Service.Metadata.Type);
                            queryable = queryable.Where<T>(Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Property(parameter, property.ClrName), Expression.Constant(result)), parameter));
                        }
                        else if (property.CustomType == "Entity")
                        {
                            searchItem.Contains = valueProvider.GetValue<string>("Search." + keys[i].Key);
                            ParameterExpression parameter = Expression.Parameter(Service.Metadata.Type);
                            Expression expression = Expression.Property(Expression.Property(parameter, property.ClrName), EntityDescriptor.GetMetadata(property.ClrType).DisplayProperty.ClrName);
                            expression = Expression.Call(expression, typeof(string).GetMethod("Contains"), Expression.Constant(searchItem.Contains));
                            queryable = queryable.Where<T>(Expression.Lambda<Func<T, bool>>(expression, parameter));
                        }
                        break;
                    default:
                        if (property.ClrType == typeof(string))
                        {
                            searchItem.Contains = valueProvider.GetValue<string>("Search." + keys[i].Key);
                            ParameterExpression parameter = Expression.Parameter(Service.Metadata.Type);
                            Expression expression = Expression.Property(parameter, property.ClrName);
                            expression = Expression.Call(expression, typeof(string).GetMethod("Contains"), Expression.Constant(searchItem.Contains));
                            queryable = queryable.Where<T>(Expression.Lambda<Func<T, bool>>(expression, parameter));
                        }
                        break;
                }
                if (searchItem.Contains != null || searchItem.Enum != null || searchItem.Equal.HasValue || searchItem.Lessthan.HasValue || searchItem.LessthanDate.HasValue || searchItem.Morethan.HasValue || searchItem.MorethanDate.HasValue)
                    searchItem.Name = property.Name;
                if (searchItem.Name != null)
                    searchItems.Add(searchItem);

                e.Queryable = queryable;
            }

            context.DomainContext.DataBag.SearchItem = searchItems.ToArray();
#if NET451
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
    }
}
