using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace System.ComponentModel
{
    /// <summary>
    /// 视图模型。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewModel<T> : NotifyBase, IViewModel<T>
    {
        /// <summary>
        /// 实例化视图模型。
        /// </summary>
        /// <param name="queryable">查询体。</param>
        public ViewModel(IAsyncQueryable<T> queryable) : this(queryable, 1, 0) { }

        /// <summary>
        /// 实例化视图模型。
        /// </summary>
        /// <param name="queryable">查询体。</param>
        /// <param name="page">当前页。</param>
        /// <param name="size">每页显示数量。</param>
        public ViewModel(IAsyncQueryable<T> queryable, int page, int size)
        {
            if (page < 1)
                throw new ArgumentException("不能小于1。", "page");
            if (size < 0)
                throw new ArgumentException("不能小于0。", "size");
            ViewButtons = new IViewButton[0];
            ItemButtons = new IItemButton[0];
            CurrentSize = size;
            PageSizeOption = Pagination.DefaultPageSizeOption;
            Queryable = queryable ?? throw new ArgumentNullException("queryable");
        }

        private IAsyncQueryable<T> _Queryable;
        /// <inheritdoc />
        public IAsyncQueryable<T> Queryable
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
        public int[] PageSizeOption { get { return (int[])GetValue(); } set { SetValue(value); } }

        /// <inheritdoc />
        public int TotalPage { get { return (int)GetValue(); } set { SetValue(value); } }

        /// <inheritdoc />
        public int CurrentSize { get { return (int)GetValue(); } set { SetValue(value); } }

        /// <inheritdoc />
        public int CurrentPage { get { return (int)GetValue(); } set { SetValue(value); } }

        /// <inheritdoc />
        public IViewButton[] ViewButtons { get; set; }

        /// <inheritdoc />
        public IItemButton[] ItemButtons { get; set; }

        object[] IViewModel.Items { get { return Items as object[]; } }

        /// <inheritdoc />
        public T[] Items { get { return (T[])GetValue(); } set { SetValue(value); } }

        /// <inheritdoc />
        public int TotalCount { get { return (int)GetValue(); } set { SetValue(value); } }

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
