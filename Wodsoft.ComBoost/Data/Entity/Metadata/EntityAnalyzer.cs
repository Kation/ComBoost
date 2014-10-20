using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Entity.Metadata
{
    /// <summary>
    /// Entity analyzer.
    /// </summary>
    public static class EntityAnalyzer
    {
        private static Dictionary<Type, EntityMetadata> _Metadata;

        static EntityAnalyzer()
        {
            _Metadata = new Dictionary<Type, EntityMetadata>();
        }

        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        /// <param name="type">Type of entity.</param>
        /// <returns>Return entity metadata. If find any error when analyze will return null.</returns>
        public static EntityMetadata GetMetadata(Type type)
        {
            while (type.Assembly.IsDynamic)
                type = type.BaseType;
            lock (_Metadata)
                if (!_Metadata.ContainsKey(type))
                    _Metadata.Add(type, BuildMetadata(type));
            return _Metadata[type];
        }

        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <returns>Return entity metadata. If find any error when analyze will return null.</returns>
        public static EntityMetadata GetMetadata<TEntity>()
        {
            return GetMetadata(typeof(TEntity));
        }

        private static EntityMetadata BuildMetadata(Type type)
        {
            EntityMetadata metadata = new EntityMetadata(type);
            return metadata;
        }
    }
}
