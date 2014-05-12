using System;
using System.Collections.Generic;
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
        /// Get or set the property metadata.
        /// </summary>
        public PropertyMetadata Metadata { get; set; }
    }
}
