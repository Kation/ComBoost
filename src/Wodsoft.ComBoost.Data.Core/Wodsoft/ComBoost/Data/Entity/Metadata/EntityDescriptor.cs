using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// 实体解释器。
    /// </summary>
    public class EntityDescriptor : IEntityDescriptor
    {
        internal static IEntityDescriptor _Descriptor;

        static EntityDescriptor()
        {
            _Descriptor = new EntityDescriptor();
        }

        /// <summary>
        /// 覆盖一个全局的实体解释器。
        /// </summary>
        /// <param name="descriptor">实体解释器。</param>
        public static void OverrideDescriptor(IEntityDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            _Descriptor = descriptor;
        }

        /// <summary>
        /// 覆盖指定类型或继承类型的实体解释器。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="descriptor">实体解释器。</param>
        public static void OverrideDescriptor<T>(IEntityDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            EntityDescriptor<T>.Descriptor = descriptor;
        }

        /// <summary>
        /// 获取实体元数据。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>返回实体元数据。如果出现错误则返回空。</returns>
        public static IEntityMetadata GetMetadata(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            IEntityDescriptor descriptor = (IEntityDescriptor)typeof(EntityDescriptor<>).MakeGenericType(type).GetProperty("Descriptor", BindingFlags.Static | BindingFlags.Public).GetValue(null);
            return descriptor.GetMetadata(type);
        }

        /// <summary>
        /// 获取实体元数据。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>返回实体元数据。如果出现错误则返回空。</returns>
        public static IEntityMetadata GetMetadata<TEntity>()
        {
            return EntityDescriptor<TEntity>.Descriptor.GetMetadata<TEntity>();
        }

        private ConcurrentDictionary<Type, IEntityMetadata> _Metadata;

        /// <summary>
        /// 实例化实体解释器。
        /// </summary>
        public EntityDescriptor()
        {
            _Metadata = new ConcurrentDictionary<Type, IEntityMetadata>();
        }

        IEntityMetadata IEntityDescriptor.GetMetadata(Type type)
        {
            return (IEntityMetadata)typeof(EntityDescriptor<>).MakeGenericType(typeof(Type)).GetProperty("Metadata").GetValue(null);
        }

        IEntityMetadata IEntityDescriptor.GetMetadata<T>()
        {
            return EntityDescriptor<T>.Metadata;
        }

        /// <summary>
        /// 初始化程序集实体元数据。
        /// </summary>
        /// <param name="assembly"></param>
        public static void InitMetadata(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t =>
            {
                var info = t.GetTypeInfo();
                if (!info.IsClass || !typeof(IEntity).IsAssignableFrom(t))
                    return false;
                return true;
            }))
                GetMetadata(type);
        }
    }

    internal class EntityDescriptor<T>
    {
        private static IEntityDescriptor? _Descriptor;
        public static IEntityDescriptor Descriptor
        {
            get
            {
                return _Descriptor ?? EntityDescriptor._Descriptor;
            }
            internal set
            {
                _Descriptor = value;
            }
        }

        private static IEntityMetadata? _Metadata;
        public static IEntityMetadata Metadata
        {
            get
            {
                if (_Metadata == null)
                {
                    var type = typeof(T);
                    while (type.Assembly.IsDynamic)
                        type = type.BaseType;
                    var metadataField = type.GetField("Metadata", BindingFlags.Static | BindingFlags.Public);
                    if (metadataField != null)
                        _Metadata = (IEntityMetadata)metadataField.GetValue(null);
                    else
                        _Metadata = new ClrEntityMetadata(type);
                    foreach (var interfaceType in type.GetInterfaces().Where(t => t != typeof(IEntity) && t.GetTypeInfo().GetCustomAttribute<EntityInterfaceAttribute>() != null && typeof(IEntity).IsAssignableFrom(t)))
                    {
                        typeof(EntityDescriptor<T>).MakeGenericType(interfaceType).GetProperty(nameof(Metadata), BindingFlags.Public | BindingFlags.Static).SetValue(null, _Metadata);
                    }
                }
                return _Metadata;
            }
            internal set
            {
                _Metadata = value;
            }
        }
    }
}
