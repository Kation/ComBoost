using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 实现了IEntity接口的实体基类。
    /// </summary>
    public abstract class EntityBase : IEntity
    {
        /// <summary>
        /// 获取或设置创建日期。
        /// </summary>
        [Hide]
        [Display(Name = "创建日期", Order = 100)]
        public virtual DateTime CreateDate { get; set; }

        /// <summary>
        /// 获取或设置编辑日期。
        /// </summary>
        [Hide]
        [Display(Name = "编辑日期", Order = 200)]
        public virtual DateTime EditDate { get; set; }

        /// <summary>
        /// 获取或设置主键Id。
        /// </summary>
        [Hide]
        [Key]
        public virtual Guid Index { get; set; }

        object IEntity.Index { get { return Index; } set { Index = (Guid)value; } }

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

        /// <summary>
        /// 创建完毕。
        /// 通常于向数据库插入之前调用。
        /// </summary>
        public virtual void OnCreateCompleted()
        {
            Index = Guid.NewGuid();
            CreateDate = DateTime.Now;
            EditDate = CreateDate;
        }

        /// <summary>
        /// 编辑完毕时。
        /// 通常于向数据库更新之前调用。
        /// </summary>
        public virtual void OnEditCompleted()
        {
            EditDate = DateTime.Now;
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

        IEntityQueryContext<IEntity> IEntity.EntityContext { get; set; }

        /// <summary>
        /// 获取是否是新创建的实体。
        /// </summary>
        bool IEntity.IsNewCreated { get { return Index == Guid.Empty; } }

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
