using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainDistributedModule
    {
        void ConfigureDistributedServices(IComBoostDistributedBuilder builder);
    }
}
