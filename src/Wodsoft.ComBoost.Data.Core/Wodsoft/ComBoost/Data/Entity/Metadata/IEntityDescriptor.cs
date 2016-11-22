using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// 实体解释器。
    /// </summary>
    public interface IEntityDescriptor
    {
        /// <summary>
        /// 获取实体元数据。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>返回实体元数据。</returns>
        IEntityMetadata GetMetadata(Type type);
    }
}
