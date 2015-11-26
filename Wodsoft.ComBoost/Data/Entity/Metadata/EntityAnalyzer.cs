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
    public class EntityAnalyzer : IEntityAnalyzer
    {
        private static IEntityAnalyzer _Analyzer;

        static EntityAnalyzer()
        {
            _Analyzer = new EntityAnalyzer();
        }

        /// <summary>
        /// Override a global entity analyzer.
        /// </summary>
        /// <param name="analyzer"></param>
        public static void OverrideAnalyzer(IEntityAnalyzer analyzer)
        {
            if (analyzer == null)
                throw new ArgumentNullException("analyzer");
            _Analyzer = analyzer;
        }

        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        /// <param name="type">Type of entity.</param>
        /// <returns>Return entity metadata. If find any error when analyze will return null.</returns>
        public static IEntityMetadata GetMetadata(Type type)
        {
            return _Analyzer.GetMetadata(type);
        }

        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <returns>Return entity metadata. If find any error when analyze will return null.</returns>
        public static IEntityMetadata GetMetadata<TEntity>()
        {
            return GetMetadata(typeof(TEntity));
        }

        private Dictionary<Type, IEntityMetadata> _Metadata;

        /// <summary>
        /// Initialize entity analyzer.
        /// </summary>
        public EntityAnalyzer()
        {
            _Metadata = new Dictionary<Type, IEntityMetadata>();
        }

        IEntityMetadata IEntityAnalyzer.GetMetadata(Type type)
        {
            while (type.Assembly.IsDynamic)
                type = type.BaseType;
            lock (_Metadata)
                if (!_Metadata.ContainsKey(type))
                {
                    var metadataField = type.GetField("Metadata", BindingFlags.Static | BindingFlags.Public);
                    if (metadataField != null)
                        _Metadata.Add(type, (IEntityMetadata)metadataField.GetValue(null));
                    else
                        _Metadata.Add(type, new ClrEntityMetadata(type));
                }
            return _Metadata[type];
        }
    }
}
