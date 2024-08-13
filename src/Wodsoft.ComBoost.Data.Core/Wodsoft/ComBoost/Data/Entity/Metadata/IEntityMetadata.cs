using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data.Entity.Metadata
{
    /// <summary>
    /// 实体元数据。
    /// </summary>
    public interface IEntityMetadata
    {
        /// <summary>
        /// 获取实体类型。
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// 获取主键属性。
        /// </summary>
        IReadOnlyList<IPropertyMetadata> KeyProperties { get; }

        /// <summary>
        /// 获取实体名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取默认显示属性。
        /// </summary>
        IPropertyMetadata? DisplayProperty { get; }

        /// <summary>
        /// 获取默认排序属性。
        /// </summary>
        IPropertyMetadata SortProperty { get; }

        /// <summary>
        /// 获取默认父级属性。
        /// </summary>
        IPropertyMetadata? ParentProperty { get; }

        /// <summary>
        /// 获取是否倒序排序。
        /// </summary>
        bool IsSortDescending { get; }

        /// <summary>
        /// 获取所有属性。
        /// </summary>
        IReadOnlyList<IPropertyMetadata> Properties { get; }

        ///// <summary>
        ///// 获取列表属性。
        ///// </summary>
        //IReadOnlyList<IPropertyMetadata> ViewProperties { get; }

        ///// <summary>
        ///// 获取创建属性。
        ///// </summary>
        //IReadOnlyList<IPropertyMetadata> CreateProperties { get; }

        ///// <summary>
        ///// 获取编辑属性。
        ///// </summary>
        //IReadOnlyList<IPropertyMetadata> EditProperties { get; }

        ///// <summary>
        ///// 获取搜索属性。
        ///// </summary>
        //IReadOnlyList<IPropertyMetadata> SearchProperties { get; }

        ///// <summary>
        ///// 获取详情属性。
        ///// </summary>
        //IReadOnlyList<IPropertyMetadata> DetailProperties { get; }

        ///// <summary>
        ///// 获取是否允许匿名访问。
        ///// </summary>
        //bool AllowAnonymous { get; }

        ///// <summary>
        ///// 获取列表角色。
        ///// </summary>
        //IEnumerable<object> ViewRoles { get; }

        ///// <summary>
        ///// 获取新增角色。
        ///// </summary>
        //IEnumerable<object> AddRoles { get; }

        ///// <summary>
        ///// 获取编辑角色。
        ///// </summary>
        //IEnumerable<object> EditRoles { get; }

        ///// <summary>
        ///// 获取删除角色。
        ///// </summary>
        //IEnumerable<object> RemoveRoles { get; }

        /// <summary>
        /// 获取属性。
        /// </summary>
        /// <param name="name">属性名。</param>
        /// <returns>返回元数据。如果不存在则返回空。</returns>
        IPropertyMetadata? GetProperty(string name);

        /// <summary>
        /// 获取数据字典。
        /// </summary>
        dynamic DataBag { get; }
    }
}
