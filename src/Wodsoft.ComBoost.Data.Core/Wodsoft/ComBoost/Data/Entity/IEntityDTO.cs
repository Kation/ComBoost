using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntityDTO
    {
        /// <summary>
        /// 获取或设置创建时间。
        /// </summary>
        DateTimeOffset CreationDate { get; set; }

        /// <summary>
        /// 获取或设置修改时间。
        /// </summary>
        DateTimeOffset ModificationDate { get; set; }
    }
}
