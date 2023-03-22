using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Linq
{
    public class UpdateCaller<TSource>
    {
        /// <summary>
        /// 更新属性。
        /// </summary>
        /// <typeparam name="TProperty">属性类型。</typeparam>
        /// <param name="propertySelector">属性选择器。</param>
        /// <param name="valueProvider">更新值。</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public UpdateCaller<TSource> Property<TProperty>(Func<TSource, TProperty> propertySelector, Func<TSource, TProperty> valueProvider)
        {
            throw new InvalidOperationException("仅用于Lambda表达式，不能直接调用。");
        }
    }
}
