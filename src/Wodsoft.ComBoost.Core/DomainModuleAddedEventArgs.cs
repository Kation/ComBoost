using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainModuleAddedEventArgs : EventArgs
    {
        public DomainModuleAddedEventArgs(IDomainModule module)
        {
            Module = module;
        }

        public IDomainModule Module { get; }
    }
}
