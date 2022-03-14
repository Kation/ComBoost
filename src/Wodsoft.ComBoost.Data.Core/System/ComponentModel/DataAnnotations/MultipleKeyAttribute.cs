using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MultipleKeyAttribute : Attribute
    {
        public MultipleKeyAttribute(params string[] keys)
        {
            Keys = keys;
        }

        public string[] Keys { get; }
    }
}
