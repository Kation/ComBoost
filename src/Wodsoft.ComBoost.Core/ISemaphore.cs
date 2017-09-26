using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 信号器。
    /// </summary>
    public interface ISemaphore
    {
        /// <summary>
        /// 异步进入信号。
        /// </summary>
        /// <returns>返回异步任务。</returns>
        Task EnterAsync();

        /// <summary>
        /// 异步尝试进入信号。
        /// </summary>
        /// <returns>返回异步任务。</returns>
        Task<bool> TryEnterAsync();

        /// <summary>
        /// 异步进入信号。
        /// </summary>
        /// <param name="timeout">超时时间。</param>
        /// <returns>返回异步任务。</returns>
        Task<bool> EnterAsync(int timeout);

        /// <summary>
        /// 异步退出信号。
        /// </summary>
        /// <returns>返回异步任务。</returns>
        Task ExitAsync();
    }
}
