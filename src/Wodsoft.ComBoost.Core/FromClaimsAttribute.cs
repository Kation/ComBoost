using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 声明来源特性。
    /// </summary>
    public class FromClaimsAttribute : FromAttribute
    {
        /// <summary>
        /// 实例化声明来源特性。
        /// </summary>
        public FromClaimsAttribute() { }

        /// <summary>
        /// 实例化声明来源特性。
        /// </summary>
        /// <param name="name">声明名称。</param>
        public FromClaimsAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 获取是否必须存在值。默认为False。
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// 获取声明名称。
        /// </summary>
        public string? Name { get; }

        public override object? GetValue(IDomainContext context, ParameterInfo parameter)
        {
            if (parameter.ParameterType.IsArray)
            {
                var claims = context.User.FindAll(Name ?? parameter.Name);
                var elementType = parameter.ParameterType.GetElementType();
                if (elementType == typeof(string))
                    return claims.Select(t => t.Value).ToArray();
                else
                {
                    var converter = TypeDescriptor.GetConverter(elementType);
                    var array = Array.CreateInstance(elementType, claims.Count());
                    int i = 0;
                    foreach(var claim in claims)
                    {
                        array.SetValue(converter.ConvertFromString(claim.Value), i);
                        i++;
                    }
                    return array;
                }
            }
            else
            {
                var claim = context.User.FindFirst(Name ?? parameter.Name);
                if (claim == null)
                {
                    if (IsRequired)
                        throw new DomainServiceException(new ArgumentNullException(parameter.Name, "获取" + (Name ?? parameter.Name) + "的声明值为空。"));
                    if (parameter.ParameterType.IsValueType)
                        return Activator.CreateInstance(parameter.ParameterType);
                    return null;
                }
                if (parameter.ParameterType == typeof(string))
                    return claim.Value;
                var converter = TypeDescriptor.GetConverter(parameter.ParameterType);
                return converter.ConvertFromString(claim.Value);
            }
        }
    }
}
