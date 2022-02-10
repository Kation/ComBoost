using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public class EditModel<T> : IEditModel<T>
    {
        public EditModel(T item)
        {
            Item = item;
            ErrorMessage = new List<KeyValuePair<string, string>>();
        }

        public EditModel(IUpdateModel<T> updateModel)
        {
            Item = updateModel.Item;
            ErrorMessage = new List<KeyValuePair<string, string>>();
        }

        public T Item { get; }

        public IList<KeyValuePair<string, string>> ErrorMessage { get; }

        object IEditModel.Item => Item!;
    }
}
