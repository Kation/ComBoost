﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 实现了IEntity接口的实体基类。
    /// </summary>
    public abstract class EntityBase<T> : EntityDTOBase, IEntity<T>
    {
        /// <summary>
        /// 获取或设置创建日期。
        /// </summary>
        [Hide]
        [Display(Name = "创建日期", Order = 100)]
        public override DateTimeOffset CreationDate { get; set; }

        /// <summary>
        /// 获取或设置编辑日期。
        /// </summary>
        [Hide]
        [Display(Name = "编辑日期", Order = 200)]
        public override DateTimeOffset ModificationDate { get; set; }

        /// <summary>
        /// 获取或设置主键Id。
        /// </summary>
        [Hide]
        [Key]
        [AllowNull]
        public virtual T Id { get; set; }

        /// <summary>
        /// 获取是否允许删除。
        /// </summary>
        [Hide]
        public virtual bool IsEditAllowed { get { return true; } }

        /// <summary>
        /// 获取是否允许编辑。
        /// </summary>
        [Hide]
        public virtual bool IsRemoveAllowed { get { return true; } }

        bool IEntity.IsNewCreated { get { return IsNewCreated; } }

        /// <summary>
        /// 获取是否是新创建的实体。
        /// </summary>
        protected abstract bool IsNewCreated { get; }

        /// <summary>
        /// 创建完毕。
        /// 通常于向数据库插入之前调用。
        /// </summary>
        public virtual void OnCreateCompleted()
        {
            if (CreationDate == DateTimeOffset.MinValue)
                CreationDate = DateTimeOffset.Now;
            if (ModificationDate == DateTimeOffset.MinValue)
                ModificationDate = CreationDate;
        }

        /// <summary>
        /// 编辑完毕时。
        /// 通常于向数据库更新之前调用。
        /// </summary>
        public virtual void OnEditCompleted()
        {
            ModificationDate = DateTime.Now;
        }

        /// <summary>
        /// 创建时。
        /// 通常于创建实体实例时调用。
        /// </summary>
        public virtual void OnCreating() { }

        /// <summary>
        /// 编辑时。
        /// 通常于在用户编辑之前调用。
        /// </summary>
        public virtual void OnEditing() { }

        /// <summary>
        /// 获取实体名称。
        /// </summary>
        /// <returns>返回实体名称。</returns>
        public override string ToString()
        {
            var metadata = EntityDescriptor.GetMetadata(GetType());
            if (metadata.DisplayProperty == null)
                return base.ToString();
            object value = metadata.DisplayProperty.GetValue(this);
            if (value == null)
                return "";
            else
                return value.ToString();
        }
    }
}
