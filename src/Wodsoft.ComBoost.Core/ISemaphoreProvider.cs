using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 信号量提供器。
    /// </summary>
    public interface ISemaphoreProvider
    {
        /// <summary>
        /// 获取信号量。
        /// </summary>
        /// <param name="name">信号名称。</param>
        /// <returns>返回信号量。</returns>
        ISemaphore GetSemaphore(string name);
    }
}
