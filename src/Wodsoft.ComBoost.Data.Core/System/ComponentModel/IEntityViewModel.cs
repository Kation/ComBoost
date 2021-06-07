using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace System.ComponentModel
{
    /// <summary>
    /// 实体视图模型接口。
    /// </summary>
    public interface IEntityViewModel : IViewModel
    {
        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        IEntityMetadata Metadata { get; }

        /// <summary>
        /// Get the viewlist headers.
        /// </summary>
        IEnumerable<IPropertyMetadata> Properties { get; }
        
        /// <summary>
        /// Get the parent models.
        /// </summary>
        EntityParentModel[] Parent { get; }

        /// <summary>
        /// Get the search items.
        /// </summary>
        EntitySearchItem[] SearchItem { get; }           
    }

    /// <summary>
    /// 实体视图模型接口。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public interface IEntityViewModel<out T> : IEntityViewModel, IViewModel<T>
    {

    }
}
