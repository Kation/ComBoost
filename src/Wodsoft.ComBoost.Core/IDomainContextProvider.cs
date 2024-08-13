using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainContextProvider
    {
        bool CanProvide { get; }

        IDomainContext GetContext();
    }
}
