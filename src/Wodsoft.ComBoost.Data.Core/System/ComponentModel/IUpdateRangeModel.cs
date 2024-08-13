using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public interface IUpdateRangeModel
    {
        bool IsSuccess { get; }

        IReadOnlyList<IUpdateRangeModelItem> Items { get; }
    }

    public interface IUpdateRangeModel<T> : IUpdateRangeModel
    {
        new IReadOnlyList<IUpdateRangeModelItem<T>> Items { get; }
    }

    public interface IUpdateRangeModelItem
    {
        IList<KeyValuePair<string, string>> ErrorMessage { get; }
    }

    public interface IUpdateRangeModelItem<T> : IUpdateRangeModelItem
    {
        T? Value { get; }
    }
}
