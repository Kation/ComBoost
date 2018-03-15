using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 实体来源特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromEntityAttribute : FromAttribute
    {
        /// <summary>
        /// 实例化实体来源特性。默认必须值。
        /// </summary>
        public FromEntityAttribute() { IsRequired = true; }

        /// <summary>
        /// 实例化实体来源特性。
        /// </summary>
        /// <param name="isRequired">是否必须。</param>
        public FromEntityAttribute(bool isRequired) { IsRequired = isRequired; }

        /// <summary>
        /// 获取自定义来源名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取是否为必须。
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="executionContext">领域执行上下文。</param>
        /// <param name="parameter">参数信息。</param>
        /// <returns>返回值。</returns>
        public override object GetValue(IDomainExecutionContext executionContext, ParameterInfo parameter)
        {
            var metadata = EntityDescriptor.GetMetadata(parameter.ParameterType);
            IValueProvider provider = executionContext.DomainContext.GetRequiredService<IValueProvider>();
            var keyType = metadata.KeyType;
            if (keyType.GetTypeInfo().IsValueType)
                keyType = typeof(Nullable<>).MakeGenericType(keyType);
            object value = provider.GetValue(Name ?? parameter.Name, keyType);
            if (value == null)
                if (IsRequired)
                    throw new DomainServiceException(new ArgumentNullException(parameter.Name, "获取" + (Name ?? parameter.Name) + "实体的值为空。"));
                else
                    return null;
            var databaseContext = executionContext.DomainContext.GetRequiredService<IDatabaseContext>();
            dynamic entityContext;
            var type = metadata.Type;
            entityContext = typeof(IDatabaseContext).GetMethod("GetContext").MakeGenericMethod(type).Invoke(databaseContext, new object[0]);
            object entity = entityContext.GetAsync(value).Result;
            if (IsRequired && entity == null)
                throw new EntityNotFoundException(parameter.ParameterType, value);
            return entity;
        }
    }
}
