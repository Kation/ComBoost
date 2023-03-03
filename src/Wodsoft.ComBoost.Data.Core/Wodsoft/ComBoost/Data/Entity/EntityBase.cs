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
            if (Id == Guid.Empty)
                Id = Guid.NewGuid();
        }

        /// <summary>
        /// 获取是否是新创建的实体。
        /// </summary>
        [Obsolete("No used anymore.")]
        protected override bool IsNewCreated { get { return Id == Guid.Empty; } }
    }
}
