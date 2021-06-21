using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// 视图模型接口。
    /// </summary>
    public interface IViewModel : IPagination
    {
        /// <summary>
        /// 获取内容项。
        /// </summary>
        IReadOnlyList<object> Items { get; }

        /// <summary>
        /// 设置每页显示个数选项。
        /// </summary>
        new int[] PageSizeOption { get; set; }

        /// <summary>
        /// 设置当前页。
        /// </summary>
        /// <param name="page">当前页。</param>
        void SetPage(int page);

        /// <summary>
        /// 设置每页显示内容数量。
        /// </summary>
        /// <param name="size">每页内容数量。</param>
        void SetSize(int size);

        /// <summary>
        /// 更新当前总页数。
        /// </summary>
        Task UpdateTotalPageAsync();

        /// <summary>
        /// 更新当前页内容。
        /// </summary>
        Task UpdateItemsAsync();
    }

    public interface IViewModel<out T> : IViewModel
        where T : class
    {
        ///// <summary>
        ///// 获取当前查询体。
        ///// </summary>
        //IAsyncQueryable<T> Queryable { get; }

        /// <summary>
        /// 获取内容项。
        /// </summary>
        new IReadOnlyList<T> Items { get; }
    }
}
