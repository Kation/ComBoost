using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public class EditModel<T> : NotifyBase, IEditModel<T>
    {
        public EditModel() { }

        public EditModel(T item)
        {
            Item = item;
        }

        public T Item { get { return (T)GetValue(); } set { SetValue(value); } }

        object IEditModel.Item => Item;
    }
}
