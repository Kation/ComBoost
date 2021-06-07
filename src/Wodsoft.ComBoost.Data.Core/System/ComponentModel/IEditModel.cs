using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public interface IEditModel
    {
        /// <summary>
        /// Get or set the item to edit.
        /// </summary>
        object Item { get; }
    }

    public interface IEditModel<out T>: IEditModel
    {
        /// <summary>
        /// Get or set the properties to edit.
        /// </summary>
        new T Item { get; }
    }
}
