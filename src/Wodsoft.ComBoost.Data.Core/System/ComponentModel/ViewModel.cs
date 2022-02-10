using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Linq;

namespace System.ComponentModel
{
    /// <summary>
    /// 视图模型。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewModel<T> : IViewModel<T>
        where T : class
    {
        /// <summary>
        /// 实例化视图模型。
        /// </summary>
        /// <param name="queryable">查询体。</param>
        public ViewModel(IQueryable<T> queryable) : this(queryable, 1, 0) { }

        /// <summary>
        /// 实例化视图模型。
        /// </summary>
        /// <param name="queryable">查询体。</param>
        /// <param name="page">当前页。</param>
        /// <param name="size">每页显示数量。</param>
        public ViewModel(IQueryable<T> queryable, int page, int size)
        {
            if (page < 1)
                throw new ArgumentException("不能小于1。", "page");
            if (size < 0)
                throw new ArgumentException("不能小于0。", "size");
            CurrentSize = size;
            PageSizeOption = Pagination.DefaultPageSizeOption;
            _Queryable = queryable ?? throw new ArgumentNullException("queryable");
            CurrentPage = page;
        }

        private IQueryable<T> _Queryable;
        /// <inheritdoc />
        protected IQueryable<T> Queryable
        {
            get
            {
                return _Queryable;
            }
            set
            {
                _Queryable = value ?? throw new ArgumentNullException("value");
                CurrentPage = 1;
            }
        }

        /// <inheritdoc />
        public int[] PageSizeOption { get; set; }

        /// <inheritdoc />
        public int TotalPage { get; set; }

        /// <inheritdoc />
        public int CurrentSize { get; set; }

        /// <inheritdoc />
        public int CurrentPage { get; set; }

        IReadOnlyList<object>? IViewModel.Items { get { return Items; } }

        /// <inheritdoc />
        public IReadOnlyList<T>? Items { get; set; }

        /// <inheritdoc />
        public int TotalCount { get; set; }

        /// <inheritdoc />
        public void SetPage(int page)
        {
            if (page < 1)
                throw new ArgumentException("页数不能小于1。", "page");
            CurrentPage = page;
        }

        /// <inheritdoc />
        public void SetSize(int size)
        {
            if (size < 0)
                throw new ArgumentException("每页显示数量不能小于0。", "size");
            CurrentSize = size;
        }

        /// <inheritdoc />
        public async Task UpdateTotalPageAsync()
        {
            int total = await Queryable.CountAsync();
            TotalCount = total;
            TotalPage = (int)Math.Ceiling(total / (double)CurrentSize);
            if (TotalPage == 0)
                TotalPage = 1;
            if (CurrentPage > TotalPage)
                CurrentPage = TotalPage;
        }

        /// <inheritdoc />
        public async Task UpdateItemsAsync()
        {
            if (CurrentSize > 0)
                Items = await Queryable.Skip((CurrentPage - 1) * CurrentSize).Take(CurrentSize).ToArrayAsync();
            else
                Items = await Queryable.ToArrayAsync();
        }
    }
}
