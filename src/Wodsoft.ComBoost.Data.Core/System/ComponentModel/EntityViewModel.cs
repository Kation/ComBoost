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
    public class EntityViewModel<T> : ViewModel<T>, IEntityViewModel<T>
        where T : class
    {
        /// <summary>
        /// Initialize entity view model.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        public EntityViewModel(IAsyncQueryable<T> queryable) : this(queryable, 1, Pagination.DefaultPageSize) { }

        /// <summary>
        /// Initialize entity view model.
        /// </summary>
        /// <param name="queryable">Queryable of entity.</param>
        /// <param name="page">Current page.</param>
        /// <param name="size">Current page size.</param>
        public EntityViewModel(IAsyncQueryable<T> queryable, int page, int size) : base(queryable, page, size)
        {
            Metadata = EntityDescriptor.GetMetadata<T>();
            ViewButtons = new IViewButton[0];
            ItemButtons = new IItemButton[0];
        }
        
        /// <summary>
        /// Get or set the metadata of entity.
        /// </summary>
        public IEntityMetadata Metadata { get; set; }

        /// <summary>
        /// Get or set the viewlist headers.
        /// </summary>
        public IEnumerable<IPropertyMetadata> Properties { get; set; }

        /// <summary>
        /// Get or set the parent models.
        /// </summary>
        public EntityParentModel[] Parent { get; set; }

        /// <summary>
        /// Get or set the search items.
        /// </summary>
        public EntitySearchItem[] SearchItem { get; set; }

        /// <inheritdoc />
        public IViewButton[] ViewButtons { get; set; }

        /// <inheritdoc />
        public IItemButton[] ItemButtons { get; set; }
    }
}
