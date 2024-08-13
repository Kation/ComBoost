using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ResourceExistsException : Exception
    {
        public ResourceExistsException(string message) : base(message) { }
    }
}
