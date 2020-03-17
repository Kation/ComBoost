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
        public ViewModel(IQueryable<T> queryable) : this(queryable, 1, Pagination.DefaultPageSize) { }

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
            if (size < 1)
                throw new ArgumentException("不能小于1。", "size");
            ViewButtons = new IViewButton[0];
            ItemButtons = new IItemButton[0];
            CurrentSize = size;
            PageSizeOption = Pagination.DefaultPageSizeOption;
            Queryable = queryable ?? throw new ArgumentNullException("queryable");
        }

        private IQueryable<T> _Queryable;
        /// <inheritdoc />
        public IQueryable<T> Queryable
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
            if (page > TotalPage)
                page = TotalPage;
            CurrentPage = page;
        }

        /// <inheritdoc />
        public void SetSize(int size)
        {
            if (size < 1)
                throw new ArgumentException("每页显示数量不能小于1。", "size");
            CurrentSize = size;
            if (CurrentPage != 1)
                SetPage(1);
        }

        /// <inheritdoc />
        public async Task UpdateTotalPageAsync()
        {
            int total = await Queryable.CountAsync();
            TotalCount = total;
            TotalPage = (int)Math.Ceiling(total / (double)CurrentSize);
            if (TotalPage == 0)
                TotalPage = 1;
        }

        /// <inheritdoc />
        public async Task UpdateItemsAsync()
        {
            Items = await Linq.Queryable.Skip<T>(Queryable, (int)((CurrentPage - 1) * CurrentSize)).Take(CurrentSize).ToArrayAsync();
        }
    }
}
