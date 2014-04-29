using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// Image supported controller.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    public interface IImageController<TEntity> : IFileController<TEntity> where TEntity : EntityBase, new()
    {
        /// <summary>
        /// Get a image from property.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <param name="property">Property name.</param>
        /// <returns></returns>
        FileResult ImageToProperty(Guid id, string property);
    }
}
