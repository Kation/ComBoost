using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IDTOContext<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO>
        where TListDTO : class, IEntityDTO
        where TCreateDTO : class, IEntityDTO
        where TEditDTO : class, IEntityDTO
        where TRemoveDTO : class
    {
        IQueryable<TListDTO> Query();

        /// <summary>
        /// 将实体添加到数据库。
        /// </summary>
        /// <param name="item">要插入的实体映射对象。</param>
        Task Add(TCreateDTO item);
        /// <summary>
        /// 将多个实体添加到数据库。
        /// </summary>
        /// <param name="items">要插入的实体映射对象。</param>
        Task AddRange(IEnumerable<TCreateDTO> items);

        /// <summary>
        /// 编辑实体数据到数据库。
        /// </summary>
        /// <param name="item">要编辑的实体映射对象。</param>
        Task Update(TEditDTO item);
        /// <summary>
        /// 编辑实体数据到数据库。
        /// </summary>
        /// <param name="items">要编辑的实体映射对象。</param>
        Task UpdateRange(IEnumerable<TEditDTO> items);

        /// <summary>
        /// 从数据库删除实体。
        /// </summary>
        /// <param name="dto">要删除的实体映射对象。</param>
        Task Remove(TRemoveDTO dto);

        /// <summary>
        /// 从数据库删除实体。
        /// </summary>
        /// <param name="dtos">要删除的实体映射对象数组。</param>
        Task RemoveRange(params TRemoveDTO[] dtos);
    }
}
