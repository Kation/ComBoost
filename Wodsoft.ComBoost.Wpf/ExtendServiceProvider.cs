using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Wpf
{
    public class ExtendServiceProvider : IServiceProvider
    {
        public ExtendServiceProvider()
        {
            _Instance = new Dictionary<Type, object>();
            _Type = new Dictionary<Type, Type>();
        }

        public ExtendServiceProvider(IServiceProvider serviceProvider)
            : this()
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");
            BaseProvider = serviceProvider;
        }

        private Dictionary<Type, object> _Instance;
        private Dictionary<Type, Type> _Type;

        public IServiceProvider BaseProvider { get; private set; }

        public void RegisterInstance<T>(T item)
        {
            Type type = typeof(T);
            if (_Instance.ContainsKey(type))
                _Instance[type] = item;
            else
                _Instance.Add(type, item);
        }

        public void RegisterType<TFrom, TTo>()
            where TTo : class,TFrom
        {
            Type from = typeof(TFrom);
            if (_Type.ContainsKey(from))
                _Type[from] = typeof(TTo);
            else
                _Type.Add(from, typeof(TTo));
        }

        public object GetService(Type serviceType)
        {
            if (_Instance.ContainsKey(serviceType))
                return _Instance[serviceType];
            if (_Type.ContainsKey(serviceType))
            {
                var targetType = _Type[serviceType];
                var contructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderBy(t => t.GetParameters().Length).ToArray();
                foreach (var item in contructors)
                {
                    var parameterInfos = item.GetParameters();
                    object[] parameters = new object[parameterInfos.Length];
                    int i;
                    for (i = 0; i < parameterInfos.Length; i++)
                    {
                        try
                        {
                            parameters[i] = GetService(parameterInfos[i].ParameterType);
                        }
                        catch
                        {
                            break;
                        }
                        if (parameters[i] == null)
                            break;
                    }
                    if (i != parameterInfos.Length)
                        continue;

                    try
                    {
                        var obj = Activator.CreateInstance(targetType, parameters);
                        return obj;
                    }
                    catch
                    {

                    }
                }
            }
            if (BaseProvider == null)
                throw new NotSupportedException("Could not resolve type.");
            return BaseProvider.GetService(serviceType);
        }
    }
}
