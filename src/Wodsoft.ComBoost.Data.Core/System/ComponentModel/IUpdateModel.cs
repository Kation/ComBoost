﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace System.ComponentModel
{
    public interface IUpdateModel
    {
        bool IsSuccess { get; }

        IList<KeyValuePair<string, string>> ErrorMessage { get; }
    }

    public interface IUpdateModel<T> : IUpdateModel
    {
        T? Item { get; }
    }
}
