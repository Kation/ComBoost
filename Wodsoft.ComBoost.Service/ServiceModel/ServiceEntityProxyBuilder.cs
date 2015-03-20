using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public sealed class ServiceEntityProxyBuilder
    {
        private static Dictionary<Type, Type> _Cache;
        private static AssemblyBuilder _Assembly;
        private static ModuleBuilder _Module;

        static ServiceEntityProxyBuilder()
        {
            _Cache = new Dictionary<Type, Type>();
            _Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ComBoostDynamicProxy"), AssemblyBuilderAccess.Run);
            _Module = _Assembly.DefineDynamicModule("ComBoostDynamicProxy");            
        }

        public static Type GetProxyType<TEntity>() where TEntity : class, IEntity, new()
        {
            return GetProxyType(typeof(TEntity));
        }

        public static Type GetProxyType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (_Cache.ContainsKey(type))
                return _Cache[type];
            if (type.Assembly == Assembly.GetExecutingAssembly())
                throw new ArgumentException("Type of parameter is a proxy type already.");

            TypeBuilder builder = _Module.DefineType("Proxy_" + type.Name + "_" + Guid.NewGuid().ToString().Replace("-", ""));
            
            

            Type proxyType = builder.CreateType();
            _Cache.Add(type, proxyType);
            return proxyType;
        }

        public static TEntity CreateEntityProxy<TEntity>() where TEntity : class, IEntity, new()
        {
            Type proxyType = GetProxyType<TEntity>();
            return (TEntity)Activator.CreateInstance(proxyType);
        }

        public static object CreateEntityProxy(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            Type proxyType = GetProxyType(type);
            return Activator.CreateInstance(proxyType);
        }
    }
}
