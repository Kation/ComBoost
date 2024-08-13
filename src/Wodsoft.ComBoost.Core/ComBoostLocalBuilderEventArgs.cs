using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostLocalBuilderEventArgs : EventArgs
    {
        public ComBoostLocalBuilderEventArgs(Type type, IComBoostLocalBuilder builder)
        {
            ServiceType = type ?? throw new ArgumentNullException(nameof(type));
            Builder = builder;
        }

        public Type ServiceType { get; }

        public IComBoostLocalBuilder Builder { get; }
    }
}
