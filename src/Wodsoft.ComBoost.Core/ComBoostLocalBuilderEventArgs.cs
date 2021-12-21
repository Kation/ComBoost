using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostLocalBuilderEventArgs : EventArgs
    {
        public ComBoostLocalBuilderEventArgs(Type type)
        {
            ServiceType = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Type ServiceType { get; }
    }
}
