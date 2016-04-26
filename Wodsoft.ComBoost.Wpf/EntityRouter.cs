using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Metadata;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Wpf
{
    public class EntityRouter
    {
        private static EntityRouter _Routers;
        public static EntityRouter Routers
        {
            get
            {
                if (_Routers == null)
                    _Routers = new EntityRouter();
                return _Routers;
            }
        }

        private EntityRouter()
        {
            _Maps = new Dictionary<Type, Type>();
        }

        private Dictionary<Type, Type> _Maps;

        public virtual void Map<TEntity>()
            where TEntity : class, IEntity, new()
        {
            Map<TEntity, EntityController<TEntity>>();
        }

        public virtual void Map<TEntity, TController>()
            where TController : IEntityController<TEntity>
            where TEntity : class, IEntity, new()
        {
            Type entityType = typeof(TEntity);
            Type controllerType = typeof(TController);
            if (_Maps.ContainsKey(entityType))
                _Maps[entityType] = controllerType;
            else
                _Maps.Add(entityType, controllerType);
        }

        public virtual IEntityController<TEntity> GetController<TEntity>()
            where TEntity : class, IEntity, new()
        {
            Type entityType = typeof(TEntity);
            Type controllerType = GetControllerType<TEntity>();
            if (controllerType == null)
                throw new InvalidOperationException("The controller of entity \"" + entityType.Name + "\" not mapped.");

            return (IEntityController<TEntity>)GetControllerInstance(controllerType);
        }

        public virtual object GetController(IEntityMetadata metadata)
        {
            return GetController(metadata.Type);
        }

        public virtual object GetController(Type entityType)
        {
            Type controllerType = GetControllerType(entityType);
            if (controllerType == null)
                throw new InvalidOperationException("The controller of entity \"" + entityType.Name + "\" not mapped.");
            return GetControllerInstance(controllerType);
        }

        protected virtual object GetControllerInstance(Type controllerType)
        {
            var contructors = controllerType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderBy(t => t.GetParameters().Length).ToArray();
            foreach (var item in contructors)
            {
                var parameterInfos = item.GetParameters();
                object[] parameters = new object[parameterInfos.Length];
                int i;
                for (i = 0; i < parameterInfos.Length; i++)
                {
                    parameters[i] = EntityResolver.Current.TryResolve(parameterInfos[i].ParameterType);
                    if (parameters[i] == null)
                        break;
                }
                if (i != parameterInfos.Length)
                    continue;

                try
                {
                    var controller = Activator.CreateInstance(controllerType, parameters);
                    return controller;
                }
                catch
                {

                }
            }
            throw new NotSupportedException("Count not resolve the controller \"" + controllerType.Name + "\".");
        }

        protected virtual Type GetControllerType<TEntity>()
            where TEntity : class, IEntity, new()
        {
            Type entityType = typeof(TEntity);
            return GetControllerType(entityType);
        }

        protected virtual Type GetControllerType(Type entityType)
        {
            Type controllerType;
            if (!_Maps.TryGetValue(entityType, out controllerType))
                throw new NotSupportedException("Count not resolve the controller of entity \"" + entityType.Name + "\".");
            return controllerType;
        }
    }
}
