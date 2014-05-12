using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Interface of entity.
    /// </summary>
    public interface IEntity : IValidatableObject
    {
        /// <summary>
        /// Get or set the id of entity.
        /// </summary>
        Guid Index { get; set; }

        /// <summary>
        /// Get or set the create date of entity.
        /// </summary>
        DateTime CreateDate { get; set; }

        /// <summary>
        /// Call when entity created.
        /// </summary>
        void OnCreateCompleted();

        /// <summary>
        /// Call when entity edited.
        /// </summary>
        void OnEditCompleted();

        /// <summary>
        /// Get is the entity can remove.
        /// </summary>
        /// <returns></returns>
        bool IsRemoveAllowed { get; }

        /// <summary>
        /// Get is the entity can edit.
        /// </summary>
        /// <returns></returns>
        bool IsEditAllowed { get; }
    }
}
