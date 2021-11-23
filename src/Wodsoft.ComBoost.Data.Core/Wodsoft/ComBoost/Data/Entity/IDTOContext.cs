using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IDTOContext<TKey, TListDTO, TCreateDTO, TEditDTO>
        where TListDTO : IEntityDTO<TKey>
        where TCreateDTO : IEntityDTO<TKey>
        where TEditDTO : IEntityDTO<TKey>
    {
        IQueryable<TListDTO> Query();

        /// <summary>
        /// 将实体添加到数据库。
        /// </summary>
        /// <param name="item">要插入的实体。</param>
        Task Add(TCreateDTO item);
        /// <summary>
        /// 将多个实体添加到数据库。
        /// </summary>
        /// <param name="items">要插入的实体。</param>
        Task AddRange(IEnumerable<TCreateDTO> items);

        /// <summary>
        /// 编辑实体数据到数据库。
        /// </summary>
        /// <param name="item">要编辑的实体。</param>
        Task Update(TEditDTO item);
        /// <summary>
        /// 编辑实体数据到数据库。
        /// </summary>
        /// <param name="items">要编辑的实体。</param>
        Task UpdateRange(IEnumerable<TEditDTO> items);

        /// <summary>
        /// 从数据库删除实体。
        /// </summary>
        /// <param name="id">要删除的实体Id。</param>
        Task Remove(TKey id);
        /// <summary>
        /// 从数据库删除实体。
        /// </summary>
        /// <param name="keys">要删除的实体Id数组。</param>
        Task RemoveRange(params TKey[] keys);
    }
}
