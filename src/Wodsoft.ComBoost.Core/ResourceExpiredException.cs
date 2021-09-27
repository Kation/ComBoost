using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ResourceExpiredException : Exception
    {
        public ResourceExpiredException(string message) : base(message) { }
    }
}
