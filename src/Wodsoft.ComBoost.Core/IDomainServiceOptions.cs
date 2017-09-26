using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务选项。
    /// </summary>
    public interface IDomainServiceOptions
    {
        /// <summary>
        /// 设置选项。
        /// </summary>
        /// <param name="optionType">选项类型。</param>
        /// <param name="option">选项。</param>
        void SetOption(Type optionType, object option);

        /// <summary>
        /// 获取选项。
        /// </summary>
        /// <param name="optionType">选项类型。</param>
        /// <returns>返回选项。</returns>
        object GetOption(Type optionType);

        /// <summary>
        /// 移除选项。
        /// </summary>
        /// <param name="optionType">选项类型。</param>
        void RemoveOption(Type optionType);
    }
}
