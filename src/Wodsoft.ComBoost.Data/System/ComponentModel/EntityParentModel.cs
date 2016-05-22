using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Entity parent model.
    /// </summary>
    public class EntityParentModel
    {
        /// <summary>
        /// Get or set the name of parent.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set the id of parent.
        /// </summary>
        public Guid Index { get; set; }

        /// <summary>
        /// Get or set the path of parent to entity.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Get or set the parent selected.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Get or set the parent tree opened.
        /// </summary>
        public bool IsOpened { get; set; }

        /// <summary>
        /// Get or set the subtree of parent models.
        /// </summary>
        public EntityParentModel[] Items { get; set; }
    }
}
