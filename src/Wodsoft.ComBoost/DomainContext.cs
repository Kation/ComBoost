using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Wodsoft.ComBoost
{
    public class DomainContext : IDomainContext
    {
        private IServiceProvider _ServiceProvider;

        public DomainContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            _ServiceProvider = serviceProvider;
        }

        private dynamic _DataBag;
        public virtual dynamic DataBag
        {
            get
            {
                if (_DataBag == null)
                    _DataBag = new System.Dynamic.ExpandoObject();
                return _DataBag;
            }
        }

        public virtual object Result { get; set; }

        public virtual object GetService(Type serviceType)
        {
            return _ServiceProvider.GetService(serviceType);
        }
    }
}
