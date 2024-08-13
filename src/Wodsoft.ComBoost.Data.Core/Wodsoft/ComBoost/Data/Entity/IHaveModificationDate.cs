using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IHaveModificationDate
    {
        /// <summary>
        /// 获取或设置修改时间。
        /// </summary>
        DateTimeOffset ModificationDate { get; set; }
    }
}
