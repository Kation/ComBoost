using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public interface IUpdateRangeModel
    {
        bool IsSuccess { get; }
    }

    public interface IUpdateRangeModel<T> : IUpdateRangeModel
    {
        IReadOnlyList<IUpdateRangeModelItem<T>> Items { get; }
    }

    public interface IUpdateRangeModelItem<T>
    {
        T Value { get; }

        IList<KeyValuePair<string, string>> ErrorMessage { get; }
    }
}
