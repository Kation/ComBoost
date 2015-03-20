using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Expend a entity will display all property values not only a Index.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface)]
    public class ExpendEntityAttribute : Attribute
    {
    }
}
