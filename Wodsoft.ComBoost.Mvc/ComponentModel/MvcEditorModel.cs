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
    /// Mvc editor model.
    /// </summary>
    public class MvcEditorModel
    {
        /// <summary>
        /// Get or set the value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Get or set the entity.
        /// </summary>
        public IEntity Entity { get; set; }

        /// <summary>
        /// Get or set the property metadata.
        /// </summary>
        public IPropertyMetadata Metadata { get; set; }
    }
}
