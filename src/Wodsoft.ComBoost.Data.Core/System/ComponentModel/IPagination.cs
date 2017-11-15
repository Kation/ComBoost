using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// 分页接口。
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// 获取每页显示个数选项。
        /// </summary>
        int[] PageSizeOption { get; }

        /// <summary>
        /// 获取总页数。
        /// </summary>
        int TotalPage { get; }

        /// <summary>
        /// 获取当前页数。
        /// </summary>
        int CurrentPage { get; }

        /// <summary>
        /// 获取当前每页显示数量。
        /// </summary>
        int CurrentSize { get; }

        /// <summary>
        /// 获取总记录数。
        /// </summary>
        int TotalCount { get; }
    }
}
