using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

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
        IEnumerable<IPropertyMetadata> Properties { get; }

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
        [Obsolete("请使用UpdateTotalPageAsync。")]
        void UpdateTotalPage();

        /// <summary>
        /// Update total page.
        /// </summary>
        Task UpdateTotalPageAsync();

        /// <summary>
        /// Update items of current page.
        /// </summary>
        [Obsolete("请使用UpdateItemsAsync。")]
        void UpdateItems();

        /// <summary>
        /// Update items of current page.
        /// </summary>
        Task UpdateItemsAsync();
    }

    /// <summary>
    /// Entity view model interface.
    /// </summary>
    /// <typeparam name="T">Type of model.</typeparam>
    public interface IEntityViewModel<out T> : IEntityViewModel
    {
        /// <summary>
        /// Get the queryable object of entity.
        /// </summary>
        IQueryable<T> Queryable { get; }

        /// <summary>
        /// Get the items of current page.
        /// </summary>
        new T[] Items { get; }
    }
}
