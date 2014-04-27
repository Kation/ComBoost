using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace System.Data.Entity
{
    /// <summary>
    /// 带缓存的实体基类
    /// </summary>
    public abstract class CacheEntityBase : EntityBase
    {
        /// <summary>
        /// 更新时间
        /// </summary>
        [Required]
        [Hide]
        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "datetime2")]
        public virtual DateTime UpdateTime { get { return (DateTime)GetValue("UpdateTime"); } set { SetValue("UpdateTime", value); } }

        public override void OnEditCompleted()
        {
            UpdateTime = DateTime.Now;
        }
    }
}
