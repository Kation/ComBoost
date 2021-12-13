﻿using System;
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
        private static IEntityDescriptor _Descriptor;
        private static Dictionary<Type, IEntityDescriptor> _TargetedDescriptor;

        static EntityDescriptor()
        {
            _Descriptor = new EntityDescriptor();
            _TargetedDescriptor = new Dictionary<Type, IEntityDescriptor>();
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
            Type type = typeof(T);
            if (_TargetedDescriptor.ContainsKey(type))
                _TargetedDescriptor[type] = descriptor;
            else
                _TargetedDescriptor.Add(type, descriptor);
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
            if (_TargetedDescriptor.Count > 0)
            {
                var targeted = _TargetedDescriptor.Keys.FirstOrDefault(t => t.IsAssignableFrom(type));
                if (targeted != null)
                    return _TargetedDescriptor[targeted].GetMetadata(type);
            }
            return _Descriptor.GetMetadata(type);
        }

        /// <summary>
        /// 获取实体元数据。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>返回实体元数据。如果出现错误则返回空。</returns>
        public static IEntityMetadata GetMetadata<TEntity>()
        {
            return GetMetadata(typeof(TEntity));
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
            while (type.GetTypeInfo().Assembly.IsDynamic)
                type = type.GetTypeInfo().BaseType;
            IEntityMetadata metadata = _Metadata.GetOrAdd(type, s =>
            {
                var metadataField = type.GetField("Metadata", BindingFlags.Static | BindingFlags.Public);
                IEntityMetadata m;
                if (metadataField != null)
                    m = (IEntityMetadata)metadataField.GetValue(null);
                else
                    m = new ClrEntityMetadata(type);
                foreach (var interfaceType in type.GetInterfaces().Where(t => t != typeof(IEntity) && t.GetTypeInfo().GetCustomAttribute<EntityInterfaceAttribute>() != null && typeof(IEntity).IsAssignableFrom(t)))
                    _Metadata.TryAdd(interfaceType, m);
                return m;
            });
            return metadata;
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
}
