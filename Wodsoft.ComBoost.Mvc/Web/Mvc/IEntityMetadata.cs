using System;
using System.Collections.Generic;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    /// <summary>
    /// Entity metadata interface.
    /// </summary>
    public interface IEntityMetadata
    {
        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        EntityMetadata Metadata { get; }
    }
}
