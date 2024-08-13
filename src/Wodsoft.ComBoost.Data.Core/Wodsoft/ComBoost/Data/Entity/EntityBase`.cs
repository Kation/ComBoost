using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 带主键的实体基类。
    /// </summary>
    public abstract class EntityBase<T> : EntityBase, IEntity<T>
    {
        /// <summary>
        /// 获取或设置主键Id。
        /// </summary>
        [Hide]
        [Key]
        [AllowNull]
        public virtual T Id { get; set; }
    }
}
