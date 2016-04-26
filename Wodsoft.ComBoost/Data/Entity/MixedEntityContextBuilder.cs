using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Mixed entity context builder.
    /// </summary>
    public class MixedEntityContextBuilder : IEntityContextBuilder
    {
        private Dictionary<Type, IEntityContextBuilder> _Map;
        private Dictionary<Type, object> _Context;
        private List<Type> _Types;

        /// <summary>
        /// Initialize with main builder.
        /// </summary>
        /// <param name="mainBuilder"></param>
        public MixedEntityContextBuilder(IEntityContextBuilder mainBuilder)
        {
            _Map = new Dictionary<Type, IEntityContextBuilder>();
            _Context = new Dictionary<Type, object>();
            _Types = new List<Type>();
            MainBuilder = mainBuilder;
            DescriptorContext = new EntityDescriptorContext(this);
            if (mainBuilder != null)
                _Types.AddRange(mainBuilder.EntityTypes);
        }

        /// <summary>
        /// Initialize empty builder.
        /// </summary>
        public MixedEntityContextBuilder() : this(null) { }

        /// <summary>
        /// Get the main entity context builder.
        /// </summary>
        public IEntityContextBuilder MainBuilder { get; private set; }

        /// <summary>
        /// Map entities with a context builder.
        /// </summary>
        /// <param name="builder">Entity context builder.</param>
        public void MapContext(IEntityContextBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            foreach (var type in builder.EntityTypes)
            {
                if (_Context.ContainsKey(type))
                    _Context.Remove(type);
                if (_Map.ContainsKey(type))
                    _Map[type] = builder;
                else
                    _Map.Add(type, builder);
                if (!_Types.Contains(type))
                    _Types.Add(type);
            }
        }

        /// <summary>
        /// Map a entity with a context.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="context">Entity context.</param>
        public void MapContext<TEntity>(IEntityContext<TEntity> context)
            where TEntity : class, IEntity, new()
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Type type = typeof(TEntity);
            if (_Map.ContainsKey(type))
                _Map.Remove(type);
            if (!_Types.Contains(type))
                _Types.Add(type);
            if (_Context.ContainsKey(type))
                _Context[type] = context;
            else
                _Context.Add(type, context);
        }

        /// <summary>
        /// Map a entity with a context builder.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="builder">Entity context builder.</param>
        public void MapContext<TEntity>(IEntityContextBuilder builder)
            where TEntity : class, IEntity, new()
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            Type type = typeof(TEntity);
            if (_Context.ContainsKey(type))
                _Context.Remove(type);
            if (_Map.ContainsKey(type))
                _Map[type] = builder;
            else
                _Map.Add(type, builder);
            if (!_Types.Contains(type))
                _Types.Add(type);
        }

        /// <summary>
        /// Get entity context.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <returns>Return entity context.</returns>
        /// <exception cref="ArgumentException">Type of entity doesn't support.</exception>
        public IEntityContext<TEntity> GetContext<TEntity>() where TEntity : class, IEntity, new()
        {
            Type type = typeof(TEntity);
            if (_Types.Contains(type))
                throw new NotSupportedException(type.Name + " doesn't belong to this context.");
            if (_Context.ContainsKey(type))
                return (IEntityContext<TEntity>)_Map[type];
            object context;
            if (_Map.ContainsKey(type))
                context = _Map[type].GetContext(type);
            else
                context = MainBuilder.GetContext(type);
            _Context.Add(type, context);
            return (IEntityContext<TEntity>)context;
        }

        /// <summary>
        /// Get entity context.
        /// </summary>
        /// <param name="entityType">Type of entity.</param>
        /// <returns>Return entity context.</returns>
        /// <exception cref="ArgumentException">Type of entity doesn't support.</exception>
        public object GetContext(Type entityType)
        {
            if (_Types.Contains(entityType))
                throw new NotSupportedException(entityType.Name + " doesn't belong to this context.");
            if (_Context.ContainsKey(entityType))
                return _Map[entityType];
            object context;
            if (_Map.ContainsKey(entityType))
                context = _Map[entityType].GetContext(entityType);
            else
                context = MainBuilder.GetContext(entityType);
            _Context.Add(entityType, context);
            return context;
        }

        /// <summary>
        /// Get the descriptor context of builder.
        /// </summary>
        public EntityDescriptorContext DescriptorContext { get; private set; }

        /// <summary>
        /// Get data by sql query string.
        /// Only support while there is a main builder.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="sql">Sql query string.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <returns>A System.Data.Entity.Infrastructure.DbRawSqlQuery object that will execute the query when it is enumerated.</returns>
        public IEnumerable<T> Query<T>(string sql, params object[] parameters)
        {
            if (MainBuilder == null)
                throw new NotSupportedException("Can not use query while there is no main builder.");
            return MainBuilder.Query<T>(sql, parameters);
        }

        /// <summary>
        /// Dispose entity context builder.
        /// </summary>
        public void Dispose()
        {
            if (MainBuilder != null)
                MainBuilder.Dispose();
        }
        
        /// <summary>
        /// Get support entity types array.
        /// </summary>
        public Type[] EntityTypes
        {
            get { return _Types.ToArray(); }
        }
    }
}
