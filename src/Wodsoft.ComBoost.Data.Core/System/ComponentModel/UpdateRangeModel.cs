using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace System.ComponentModel
{
    public class UpdateRangeModel<T> : IUpdateRangeModel<T>
    {
        public UpdateRangeModel()
        {
            _items = new List<IUpdateRangeModelItem<T>>();
            _readOnlyItems = new ReadOnlyCollection<IUpdateRangeModelItem<T>>(_items);
        }

        private readonly List<IUpdateRangeModelItem<T>> _items;
        private readonly ReadOnlyCollection<IUpdateRangeModelItem<T>> _readOnlyItems;
        public IReadOnlyList<IUpdateRangeModelItem<T>> Items => _readOnlyItems;

        public bool IsSuccess { get; set; } = true;

        public void AddItem(T item, IList<KeyValuePair<string, string>> errorMessage)
        {
            var model = new UpdateRangeModelItem<T>(item, errorMessage);
            _items.Add(model);
            IsSuccess = IsSuccess && errorMessage.Count == 0;
        }

        public void AddItem(T item)
        {
            var model = new UpdateRangeModelItem<T>(item);
            _items.Add(model);
        }
    }

    public class UpdateRangeModelItem<T> : IUpdateRangeModelItem<T>
    {
        public UpdateRangeModelItem()
        {
            ErrorMessage = new List<KeyValuePair<string, string>>();
        }

        public UpdateRangeModelItem(T value)
        {
            Value = value;
            ErrorMessage = new List<KeyValuePair<string, string>>();
        }

        public UpdateRangeModelItem(T value, IList<KeyValuePair<string, string>> errorMessage)
        {
            Value = value;
            ErrorMessage = errorMessage;
        }

        public T Value { get; set; }

        public IList<KeyValuePair<string, string>> ErrorMessage { get; set; }
    }
}
