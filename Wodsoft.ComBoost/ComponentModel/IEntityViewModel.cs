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
    /// Entity view model interface.
    /// </summary>
    public interface IEntityViewModel : IPagination
    {
        /// <summary>
        /// Get the items of current page.
        /// </summary>
        IEntity[] Items { get; }

        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        IEntityMetadata Metadata { get; }

        /// <summary>
        /// Get the viewlist headers.
        /// </summary>
        IEnumerable<IPropertyMetadata> Headers { get; }

        /// <summary>
        /// Get the view buttons.
        /// </summary>
        IViewButton[] ViewButtons { get; }

        /// <summary>
        /// Get the item buttons.
        /// </summary>
        IEntityViewButton[] ItemButtons { get; }

        /// <summary>
        /// Get the parent models.
        /// </summary>
        EntityParentModel[] Parent { get; }

        /// <summary>
        /// Get the search items.
        /// </summary>
        EntitySearchItem[] SearchItem { get; }

        /// <summary>
        /// Set the current page.
        /// </summary>
        /// <param name="page">Page to navigate.</param>
        void SetPage(int page);
        
        /// <summary>
        /// Set the items per page.
        /// </summary>
        /// <param name="size">A number that how many items show on page.</param>
        void SetSize(int size);

        /// <summary>
        /// Update total page.
        /// </summary>
        void UpdateTotalPage();

        /// <summary>
        /// Update items of current page.
        /// </summary>
        void UpdateItems();
    }

    /// <summary>
    /// Entity view model interface.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    public interface IEntityViewModel<TEntity> : IEntityViewModel where TEntity : IEntity
    {
        /// <summary>
        /// Get the queryable object of entity.
        /// </summary>
        IQueryable<TEntity> Queryable { get; }

        /// <summary>
        /// Get the items of current page.
        /// </summary>
        new TEntity[] Items { get; }
    }
}
