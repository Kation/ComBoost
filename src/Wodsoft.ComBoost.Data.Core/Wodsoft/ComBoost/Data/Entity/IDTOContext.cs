using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IDTOContext<TList, TCreate, TEdit, TRemove>
    {
        IAsyncQueryable<TList> Query();

        /// <summary>
        /// 将实体添加到数据库。
        /// </summary>
        /// <param name="item">要插入的实体。</param>
        Task Add(TCreate item);
        /// <summary>
        /// 将多个实体添加到数据库。
        /// </summary>
        /// <param name="items">要插入的实体。</param>
        Task AddRange(IEnumerable<TCreate> items);

        /// <summary>
        /// 编辑实体数据到数据库。
        /// </summary>
        /// <param name="item">要编辑的实体。</param>
        Task Update(TEdit item);
        /// <summary>
        /// 编辑实体数据到数据库。
        /// </summary>
        /// <param name="items">要编辑的实体。</param>
        Task UpdateRange(IEnumerable<TEdit> items);

        /// <summary>
        /// 从数据库删除实体。
        /// </summary>
        /// <param name="item">要删除的实体。</param>
        Task Remove(TRemove item);
        /// <summary>
        /// 从数据库删除实体。
        /// </summary>
        /// <param name="items">要删除的实体。</param>
        Task RemoveRange(IEnumerable<TRemove> items);
    }
}
