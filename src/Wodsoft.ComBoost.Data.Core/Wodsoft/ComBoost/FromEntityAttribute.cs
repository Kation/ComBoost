﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromEntityAttribute : FromAttribute
    {
        public FromEntityAttribute() { IsRequired = true; }

        public FromEntityAttribute(bool isRequired) { IsRequired = isRequired; }

        /// <summary>
        /// 自定义来源名称。
        /// </summary>
        public string Name { get; set; }

        public bool IsRequired { get; private set; }

        public override object GetValue(IDomainExecutionContext executionContext, ParameterInfo parameter)
        {
            IValueProvider provider = executionContext.DomainContext.GetRequiredService<IValueProvider>();
            object value = provider.GetValue(Name ?? parameter.Name, EntityDescriptor.GetMetadata(parameter.ParameterType).KeyType);
            if (value == null)
                if (IsRequired)
                    throw new DomainServiceException(new ArgumentNullException(parameter.Name, "获取" + (Name ?? parameter.Name) + "实体的值为空。"));
                else
                    return null;
            var databaseContext = executionContext.DomainContext.GetRequiredService<IDatabaseContext>();
            dynamic entityContext;
            //if (parameter.ParameterType.GetTypeInfo().IsInterface)
            //    entityContext = typeof(DatabaseContextExtensions).GetMethod("GetWrappedContext").MakeGenericMethod(parameter.ParameterType).Invoke(null, new object[] { databaseContext });
            //else
            var type = EntityDescriptor.GetMetadata(parameter.ParameterType).Type;
            entityContext = typeof(IDatabaseContext).GetMethod("GetContext").MakeGenericMethod(type).Invoke(databaseContext, new object[0]);
            object entity = entityContext.GetAsync(value).Result;
            if (IsRequired && entity == null)
                throw new EntityNotFoundException(parameter.ParameterType, value);
            return entity;
        }
    }
}
