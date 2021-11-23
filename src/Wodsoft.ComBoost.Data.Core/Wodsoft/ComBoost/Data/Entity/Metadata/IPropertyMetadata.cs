using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// 属性元数据.
    /// </summary>
    public interface IPropertyMetadata
    {
        /// <summary>
        /// 获取属性名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取属性运行时名称。
        /// </summary>
        string ClrName { get; }

        /// <summary>
        /// 获取属性短名称。
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// 获取属性说明。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 获取是否是唯一值属性。
        /// </summary>
        bool IsDistinct { get; }

        /// <summary>
        /// 获取是否已展开。
        /// </summary>
        bool IsExpended { get; }

        /// <summary>
        /// 获取属性运行时类型。
        /// </summary>
        Type ClrType { get; }

        /// <summary>
        /// Get the type of property.
        /// </summary>
        CustomDataType Type { get; }

        /// <summary>
        /// 获取类型转换器。
        /// </summary>
        TypeConverter Converter { get; }

        /// <summary>
        /// Get the custom data type of property.
        /// </summary>
        string CustomType { get; }

        /// <summary>
        /// Get the property is base on upload file.
        /// </summary>
        bool IsFileUpload { get; }

        /// <summary>
        /// Get is the property must has data.
        /// </summary>
        bool IsRequired { get; }
        
        bool IsKey { get; }

        /// <summary>
        /// Get is the property hidden while creating.
        /// </summary>
        bool IsHiddenOnCreate { get; }

        /// <summary>
        /// Get is the property hidden while edit.
        /// </summary>
        bool IsHiddenOnEdit { get; }

        /// <summary>
        /// Get is the property hidden in viewlist.
        /// </summary>
        bool IsHiddenOnView { get; }

        /// <summary>
        /// Get is the property hidden in detail.
        /// </summary>
        bool IsHiddenOnDetail { get; }

        /// <summary>
        /// Get the order of property.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Get is the property search able.
        /// </summary>
        bool Searchable { get; }

        /// <summary>
        /// Get is property allow anonymous operate.
        /// </summary>
        bool AllowAnonymous { get; }

        /// <summary>
        /// Get the roles to view property.
        /// </summary>
        IEnumerable<object> ViewRoles { get; }

        /// <summary>
        /// Get the roles to edit property.
        /// </summary>
        IEnumerable<object> AddRoles { get; }

        /// <summary>
        /// Get the roles to edit property.
        /// </summary>
        IEnumerable<object> EditRoles { get; }

        /// <summary>
        /// Get is the property can get value.
        /// </summary>
        bool CanGet { get; }

        /// <summary>
        /// Get is the property can set value.
        /// </summary>
        bool CanSet { get; }

        /// <summary>
        /// Get attribute from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        T GetAttribute<T>() where T : Attribute;

        /// <summary>
        /// Get attributes from metadata.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <returns></returns>
        T[] GetAttributes<T>() where T : Attribute;

        /// <summary>
        /// Get property value from an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns></returns>
        object GetValue(object entity);

        /// <summary>
        /// Set property value to an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="value">Value.</param>
        void SetValue(object entity, object value);

        /// <summary>
        /// Try get PropertyInfo from metadata.
        /// </summary>
        /// <returns>Return value if there can be a PropertyInfo.</returns>
        PropertyInfo TryGetPropertyInfo();
    }
}
