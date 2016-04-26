using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntityResolver
    {
        private EntityResolver()
        {
            InnerResolver = new ExtendServiceProvider();
        }

        private EntityResolver(IServiceProvider serviceProvider)
        {
            InnerResolver = serviceProvider;
        }

        public IServiceProvider InnerResolver { get; private set; }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            return InnerResolver.GetService(type);
        }

        public T TryResolve<T>()
        {
            return (T)TryResolve(typeof(T));
        }

        public object TryResolve(Type type)
        {
            try
            {
                return Resolve(type);
            }
            catch
            { return null; }
        }

        private static EntityResolver _Current;
        public static EntityResolver Current
        {
            get
            {
                if (_Current == null)
                    _Current = new EntityResolver();
                return _Current;
            }
        }

        public static void SetResolver(IServiceProvider serviceProvider)
        {
            _Current = new EntityResolver(serviceProvider);
        }
    }
}
