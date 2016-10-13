using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data
{
    /// <summary>
    /// 实体详情事件参数。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityDetailEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 实例化实体详情事件参数。
        /// </summary>
        /// <param name="entity">相关实体。</param>
        /// <param name="properties">实体属性元数据。</param>
        public EntityDetailEventArgs(T entity, IPropertyMetadata[] properties)
        {
            Entity = entity;
            Properties = properties;
        }

        /// <summary>
        /// 获取相关实体。
        /// </summary>
        public T Entity { get; private set; }

        /// <summary>
        /// 获取实体属性元数据。
        /// </summary>
        public IPropertyMetadata[] Properties { get; set; }
    }
}
