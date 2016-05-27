using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域提供器接口。
    /// </summary>
    public interface IDomainProvider
    {
        TService GetService<TService>() where TService : IDomainService;

        void Override(Type serviceType, Type overrideType);

        void RegisterExtension(Type serviceType, Type extensionType);

        void UnregisterExtension(Type serviceType, Type extensionType);
    }
}
