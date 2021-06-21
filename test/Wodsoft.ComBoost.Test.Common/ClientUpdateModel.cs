using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Wodsoft.ComBoost.Test
{
    public class ClientUpdateModel<T> : IUpdateModel<T>
    {
        public T Result { get; set; }

        public bool IsSuccess { get; set; }

        public IDictionary<string, string> ErrorMessage { get; set; }
    }
}
