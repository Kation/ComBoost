using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务委托。
    /// </summary>
    /// <param name="serviceContext"></param>
    /// <returns></returns>
    public delegate Task DomainServiceDelegate(IDomainContext serviceContext);
}
