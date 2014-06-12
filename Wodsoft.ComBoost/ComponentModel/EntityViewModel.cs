using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Entity view model.
    /// </summary>
    public abstract class EntityViewModel : NotifyBase, IPagination
    {
        /// <summary>
        /// Initialize entity view model.
        /// </summary>
        protected EntityViewModel()
        {
            Buttons = new EntityViewButton[0];
        }

        /// <summary>
        /// Default number of items per page options.
        /// </summary>
        public static int[] DefaultPageSizeOption = new int[] { 10, 20, 30, 50 };

        /// <summary>
        /// Default number of items per page.
        /// </summary>
        public static int DefaultPageSize = 20;

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
        /// Get or set the items of current page.
        /// </summary>
        public IEntity[] Items { get { return (IEntity[])GetValue(); } set { SetValue(value); } }

        /// <summary>
        /// Get or set the metadata of entity.
        /// </summary>
        public EntityMetadata Metadata { get; set; }

        /// <summary>
        /// Get or set the viewlist headers.
        /// </summary>
        public PropertyMetadata[] Headers { get; set; }

        /// <summary>
        /// Get or set the viewlist buttons.
        /// </summary>
        public EntityViewButton[] Buttons { get; set; }

        /// <summary>
        /// Get or set the parent models.
        /// </summary>
        public EntityParentModel[] Parent { get; set; }

        /// <summary>
        /// Get or set the search items.
        /// </summary>
        public EntitySearchItem[] SearchItem { get; set; }

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
            UpdateTotalPage();
        }

        /// <summary>
        /// Update total page.
        /// </summary>
        public abstract void UpdateTotalPage();

        /// <summary>
        /// Update items of current page.
        /// </summary>
        public abstract void UpdateItems();
    }

    /// <summary>
    /// Entity view model.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    public class EntityViewModel<TEntity> : EntityViewModel where TEntity : IEntity
    {
        private IQueryable<TEntity> _Queryable;
        /// <summary>
        /// Get the queryable of entity.
        /// </summary>
        public IQueryable<TEntity> Queryable
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
                UpdateTotalPage();
                CurrentPage = 1;
            }
        }

        /// <summary>
        /// Initialize entity view model.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        public EntityViewModel(IQueryable<TEntity> queryable) : this(queryable, 1, DefaultPageSize) { }

        /// <summary>
        /// Initialize entity view model.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        /// <param name="page">Current page.</param>
        /// <param name="size">Current page size.</param>
        public EntityViewModel(IQueryable<TEntity> queryable, int page, int size)
        {
            if (queryable == null)
                throw new ArgumentNullException("queryable");
            if (page < 1)
                throw new ArgumentException("Can not less than 1.", "page");
            if (size < 1)
                throw new ArgumentException("Can not less than 1.", "size");
            CurrentSize = size;
            PageSizeOption = DefaultPageSizeOption;
            Metadata = EntityAnalyzer.GetMetadata<TEntity>();
            Queryable = queryable;
            UpdateTotalPage();
            SetPage(page);
        }

        /// <summary>
        /// Get or set the items of current page.
        /// </summary>
        public new TEntity[] Items
        {
            get
            {
                if (base.Items == null)
                    return null;
                return base.Items.Cast<TEntity>().ToArray();
            }
            set { base.Items = value.Cast<IEntity>().ToArray(); }
        }

        /// <summary>
        /// Update total page.
        /// </summary>
        public override void UpdateTotalPage()
        {
            int total = Queryable.Count();
            TotalPage = (int)Math.Ceiling(total / (double)CurrentSize);
            if (TotalPage == 0)
                TotalPage = 1;
        }

        /// <summary>
        /// Update items of current page.
        /// </summary>
        public override void UpdateItems()
        {
            Items = Queryable.Skip((CurrentPage - 1) * CurrentSize).Take(CurrentSize).ToArray();
        }
    }
}
