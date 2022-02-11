using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    /// <summary>
    /// Mvc editor model.
    /// </summary>
    public class MvcEditorModel
    {
        /// <summary>
        /// Get or set the value.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Get or set the entity.
        /// </summary>
        public IEntity? Entity { get; set; }

        /// <summary>
        /// Get or set the property metadata.
        /// </summary>
        public IPropertyMetadata? Metadata { get; set; }
    }
}
