using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Distributed
{
    public class ObjectChangedEventArgs<T> : DomainServiceEventArgs
        where T : class
    {
        public string[] Keys { get; set; }
    }
}
