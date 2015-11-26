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
    /// File supported controller.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    public interface IFileController<TEntity>
    {
        /// <summary>
        /// Save file.
        /// </summary>
        /// <param name="entity">Entity object.</param>
        /// <param name="metadata">Property metadata.</param>
        /// <param name="file">File base.</param>
        Task SaveFileToProperty(TEntity entity, IPropertyMetadata metadata, HttpPostedFileBase file);
    }
}
