using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务选项扩展。
    /// </summary>
    public static class DomainServiceOptionsExtensions
    {
        /// <summary>
        /// 获取选项。
        /// </summary>
        /// <typeparam name="TOption">选项类型。</typeparam>
        /// <param name="options">领域服务器选项。</param>
        /// <returns>返回选项。</returns>
        public static TOption GetOption<TOption>(this IDomainServiceOptions options)
        {
            return (TOption)options.GetOption(typeof(TOption));
        }

        /// <summary>
        /// 设置选项。
        /// </summary>
        /// <typeparam name="TOption">选项类型。</typeparam>
        /// <param name="options">领域服务器选项。</param>
        /// <param name="option">选项。</param>
        public static void SetOption<TOption>(this IDomainServiceOptions options, TOption option)
        {
            options.SetOption(typeof(TOption), option);
        }
    }
}
