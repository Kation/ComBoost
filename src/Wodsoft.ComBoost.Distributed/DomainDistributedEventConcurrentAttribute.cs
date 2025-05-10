using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DomainDistributedEventConcurrentAttribute : Attribute
    {
        public DomainDistributedEventConcurrentAttribute(uint count)
        {
            Count = count;
        }

        public uint Count { get; }
    }
}
