using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{

    /// <summary>
    /// 以Guid作为主键类型的实体基类。
    /// </summary>
    public abstract class EntityBase : EntityBase<Guid>
    {
        public override void OnCreateCompleted()
        {
            base.OnCreateCompleted();
            Index = Guid.NewGuid();
        }

        /// <summary>
        /// 获取是否是新创建的实体。
        /// </summary>
        protected override bool IsNewCreated { get { return Index == Guid.Empty; } }
    }
}
