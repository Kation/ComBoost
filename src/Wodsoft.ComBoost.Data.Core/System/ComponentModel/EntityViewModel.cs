using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace System.ComponentModel
{
    /// <summary>
    /// Entity view model.
    /// </summary>
    /// <typeparam name="T">Type of model.</typeparam>
    public class EntityViewModel<T> : NotifyBase, IEntityViewModel<T>, IEntityViewModel, IPagination
    {
        private IQueryable<T> _Queryable;
        /// <summary>
        /// Get the queryable of entity.
        /// </summary>
        public IQueryable<T> Queryable
        {
            get
            {
                return _Queryable;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _Queryable = value;
                //UpdateTotalPage();
                CurrentPage = 1;
            }
        }

        /// <summary>
        /// Initialize entity view model.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        public EntityViewModel(IQueryable<T> queryable) : this(queryable, 1, Pagination.DefaultPageSize) { }

        /// <summary>
        /// Initialize entity view model.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        /// <param name="page">Current page.</param>
        /// <param name="size">Current page size.</param>
        public EntityViewModel(IQueryable<T> queryable, int page, int size)
        {
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            if (page < 1)
                throw new ArgumentException("Can not less than 1.", "page");
            if (size < 1)
                throw new ArgumentException("Can not less than 1.", "size");
            ViewButtons = new IViewButton[0];
            ItemButtons = new IEntityViewButton[0];
            CurrentSize = size;
            PageSizeOption = Pagination.DefaultPageSizeOption;
            Metadata = EntityDescriptor.GetMetadata<T>();
            Queryable = queryable;
            //UpdateTotalPage();
            //SetPage(page);
        }

        /// <summary>
        /// Get or set the items per page options.
        /// </summary>
        public int[] PageSizeOption { get { return (int[])GetValue(); } set { SetValue(value); } }

        /// <summary>
        /// Get or set the total page.
        /// </summary>
        public int TotalPage { get { return (int)GetValue(); } set { SetValue(value); } }

        /// <summary>
        /// Get or set the items per page.
        /// </summary>
        public int CurrentSize { get { return (int)GetValue(); } set { SetValue(value); } }

        /// <summary>
        /// Get or set the current page.
        /// </summary>
        public int CurrentPage { get { return (int)GetValue(); } set { SetValue(value); } }

        /// <summary>
        /// Get or set the metadata of entity.
        /// </summary>
        public IEntityMetadata Metadata { get; set; }

        /// <summary>
        /// Get or set the viewlist headers.
        /// </summary>
        public IEnumerable<IPropertyMetadata> Properties { get; set; }

        /// <summary>
        /// Get or set the view buttons.
        /// </summary>
        public IViewButton[] ViewButtons { get; set; }

        /// <summary>
        /// Get or set the item buttons.
        /// </summary>
        public IEntityViewButton[] ItemButtons { get; set; }

        /// <summary>
        /// Get or set the parent models.
        /// </summary>
        public EntityParentModel[] Parent { get; set; }

        /// <summary>
        /// Get or set the search items.
        /// </summary>
        public EntitySearchItem[] SearchItem { get; set; }

        /// <summary>
        /// Get or set the items of current page.
        /// </summary>
        public T[] Items { get { return (T[])GetValue(); } set { SetValue(value); } }

        IEntity[] IEntityViewModel.Items { get { return Items.Cast<IEntity>().ToArray(); } }

        public int TotalCount { get; set; }

        /// <summary>
        /// Set the current page.
        /// </summary>
        /// <param name="page">Page to navigate.</param>
        public void SetPage(int page)
        {
            if (page < 1)
                throw new ArgumentException("Can not less than 1.", "size");
            if (page > TotalPage)
                page = TotalPage;
            CurrentPage = page;
        }

        /// <summary>
        /// Set the items per page.
        /// </summary>
        /// <param name="size">A number that how many items show on page.</param>
        public void SetSize(int size)
        {
            if (size < 1)
                throw new ArgumentException("Can not less than 1.", "size");
            CurrentSize = size;
            if (CurrentPage != 1)
                SetPage(1);
        }

        /// <summary>
        /// Update total page.
        /// </summary>
        [Obsolete("请使用UpdateTotalPageAsync。")]
        public virtual void UpdateTotalPage()
        {
            int total = Queryable.Count();
            TotalPage = (int)Math.Ceiling(total / (double)CurrentSize);
            if (TotalPage == 0)
                TotalPage = 1;
        }

        /// <summary>
        /// Update items of current page.
        /// </summary>
        [Obsolete("请使用UpdateItemsAsync。")]
        public virtual void UpdateItems()
        {
            Items = Queryable.Skip((CurrentPage - 1) * CurrentSize).Take(CurrentSize).ToArray();
        }

        public Task UpdateTotalPageAsync()
        {
            return Queryable.CountAsync().ContinueWith(new Action<Task<int>>(t =>
            {
                int total = t.Result;
                TotalCount = total;
                TotalPage = (int)Math.Ceiling(total / (double)CurrentSize);
                if (TotalPage == 0)
                    TotalPage = 1;
            }));
        }

        public Task UpdateItemsAsync()
        {
            return Linq.Queryable.Skip<T>(Queryable, (int)((CurrentPage - 1) * CurrentSize)).Take(CurrentSize).ToArrayAsync().ContinueWith((Action<Task<T[]>>)new Action<Task<T[]>>((Task<T[]> t) =>
            {
                Items = t.Result;
            }));
        }
    }
}
