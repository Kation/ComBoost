using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    /// <summary>
    /// 领域视图组件。
    /// </summary>
    public abstract class DomainViewComponent : ViewComponent
    {
        private IDomainServiceProvider _Provider;
        /// <summary>
        /// 获取领域服务提供器。
        /// </summary>
        protected IDomainServiceProvider DomainProvider
        {
            get
            {
                if (_Provider == null)
                    _Provider = HttpContext.RequestServices.GetRequiredService<IDomainServiceProvider>();
                return _Provider;
            }
        }

        /// <summary>
        /// 创建领域上下文。
        /// </summary>
        /// <returns></returns>
        protected virtual ViewComponentDomainContext CreateDomainContext()
        {
            return new ViewComponentDomainContext(this);
        }

        /// <summary>
        /// 创建空领域上下文。
        /// </summary>
        /// <returns></returns>
        protected virtual EmptyDomainContext CreateEmptyContext()
        {
            return new EmptyDomainContext(HttpContext.RequestServices, HttpContext.RequestAborted);
        }
    }
}
