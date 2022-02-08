using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
[assembly:InternalsVisibleTo("Wodsoft.ComBoost")]

namespace Wodsoft.ComBoost
{
    public abstract class DomainContext : IDomainContext
    {
        private IServiceProvider _ServiceProvider;

        public DomainContext(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken));
            _ServiceProvider = serviceProvider;
            Filter = new List<IDomainServiceFilter>();
            EventManager = new DomainContextEventManager(serviceProvider.GetRequiredService<IDomainServiceEventManager>());
        }

        private dynamic? _DataBag;
        public virtual dynamic DataBag
        {
            get
            {
                if (_DataBag == null)
                    _DataBag = new DomainContextDataBag();
                return _DataBag;
            }
        }

        public CancellationToken ServiceAborted { get; private set; }

        public IList<IDomainServiceFilter> Filter { get; private set; }

        public DomainServiceEventManager EventManager { get; private set; }

        public virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDomainContext) || serviceType == typeof(IServiceProvider))
                return this;
            return _ServiceProvider.GetService(serviceType);
        }
    }
}
