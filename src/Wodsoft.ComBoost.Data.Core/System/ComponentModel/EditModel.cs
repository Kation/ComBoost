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
            ErrorMessage = new List<KeyValuePair<string, string>>();
        }

        public EditModel(IUpdateModel<T> updateModel)
        {
            Item = updateModel.Item;
            ErrorMessage = updateModel.ErrorMessage;
        }

        public T Item { get { return (T)GetValue(); } set { SetValue(value); } }

        public IList<KeyValuePair<string, string>> ErrorMessage { get; }

        object IEditModel.Item => Item;
    }
}
