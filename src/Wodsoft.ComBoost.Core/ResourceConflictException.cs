using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ResourceConflictException : Exception
    {
        public ResourceConflictException(string message) : base(message) { }
    }
}
