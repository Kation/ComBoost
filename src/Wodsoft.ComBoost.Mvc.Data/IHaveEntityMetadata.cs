using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    /// <summary>
    /// Entity metadata interface.
    /// </summary>
    public interface IHaveEntityMetadata
    {
        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        IEntityMetadata Metadata { get; }
    }
}
