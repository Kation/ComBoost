using System;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics.CodeAnalysis
{
#if NETSTANDARD2_0
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    public class AllowNullAttribute : Attribute
    {

    }
#endif
}
