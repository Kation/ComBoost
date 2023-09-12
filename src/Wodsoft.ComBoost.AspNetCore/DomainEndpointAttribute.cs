using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DomainEndpointAttribute : Attribute
    {
    }
}
