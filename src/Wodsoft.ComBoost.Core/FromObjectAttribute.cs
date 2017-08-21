//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;

//namespace Wodsoft.ComBoost
//{
//    /// <summary>
//    /// 对象来源特性。
//    /// </summary>
//    public class FromObjectAttribute : FromAttribute
//    {
//        /// <summary>
//        /// 实例化值提供器来源特性。
//        /// </summary>
//        public FromObjectAttribute() : this(true) { }

//        /// <summary>
//        /// 实例化值提供器来源特性。
//        /// </summary>
//        /// <param name="isRequired">是否必须存在值。</param>
//        public FromObjectAttribute(bool isRequired) { IsRequired = isRequired; }

//        /// <summary>
//        /// 获取是否必须存在值。默认为True。
//        /// </summary>
//        public bool IsRequired { get; private set; }

//        /// <summary>
//        /// 获取或设置前缀。
//        /// </summary>
//        public string Prefix { get; set; }

//        /// <summary>
//        /// 获取值。
//        /// </summary>
//        /// <param name="executionContext">领域执行上下文。</param>
//        /// <param name="parameter">参数信息。</param>
//        /// <returns>返回值。</returns>
//        public override object GetValue(IDomainExecutionContext executionContext, ParameterInfo parameter)
//        {
//            IValueProvider valueProvider = executionContext.DomainContext.GetRequiredService<IValueProvider>();
//            var typeInfo = parameter.ParameterType.GetTypeInfo();
//            if (typeInfo.IsInterface)
//                throw new NotSupportedException("不支持接口类型。");
//            if (typeInfo.IsAbstract)
//                throw new NotSupportedException("不支持抽象类型。");
//            var type = parameter.ParameterType;
//            object obj;
//            try
//            {
//                obj = Activator.CreateInstance(type);
//            }
//            catch
//            {
//                throw new NotSupportedException("不支持有参构造函数类型。");
//            }
//            bool hasValue = false;
//            foreach (var property in type.GetProperties())
//            {
//                string name = property.Name;
//                if (Prefix != null)
//                    name = Prefix + "." + name;
//                if (valueProvider.ContainsKey(name))
//                {
//                    var value = valueProvider.GetValue(name, property.PropertyType);
//                    property.SetValue(obj, value);
//                    hasValue = true;
//                }
//            }
//            if (!hasValue)
//                if (IsRequired)
//                    throw new ArgumentNullException(parameter.Name, "获取的值为空。");
//                else
//                    return null;
//            return obj;
//        }
//    }
//}
