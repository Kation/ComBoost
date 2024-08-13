using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Wodsoft.ComBoost.Test
{
    public class ClientUpdateModel<T> : IUpdateModel<T>
    {
        public T Item { get; set; }

        public bool IsSuccess { get; set; }

        public IList<KeyValuePair<string, string>> ErrorMessage { get; set; }
    }
}
