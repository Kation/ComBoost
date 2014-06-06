using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public abstract class ValueFilter
    {
        public abstract NameValueCollection GetValues(string dependencyProperty, string dependencyValue);
    }
}
