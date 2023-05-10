using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Data
{
    public interface IEntityDomainTemplate<TDto> : IEntityDomainTemplate<TDto, TDto, TDto, TDto>
        where TDto : class
    { }

    public interface IEntityDomainTemplate<TListDTO, TCreateDTO, TEditDTO, TRemoveDTO> : IDomainTemplate
        where TListDTO : class
        where TCreateDTO : class
        where TEditDTO : class
        where TRemoveDTO : class
    {
        /// <summary>
        /// 获取列表。<br/>
        /// 领域事件：<br/>
        /// EntityQueryEventArgs&lt;TEntity&gt;<br/>
        /// EntityQueryModelCreatedEventArgs&lt;TListDTO&gt;<br/>
        /// </summary>
        /// <returns></returns>
        Task<IViewModel<TListDTO>> List();

        /// <summary>
        /// 获取列表。<br/>
        /// 领域事件：<br/>
        /// EntityQueryEventArgs&lt;TEntity&gt;<br/>
        /// EntityQueryModelCreatedEventArgs&lt;TListDTO&gt;<br/>
        /// </summary>
        /// <param name="page">页数。</param>
        /// <param name="size">分页大小。</param>
        /// <returns></returns>
        Task<IViewModel<TListDTO>> List(int page, int size);

        /// <summary>
        /// 创建实体。<br/>
        /// 领域事件：<br/>
        /// EntityMappedEventArgs&lt;TEntity, TCreateDTO&gt;<br/>
        /// EntityPreCreateEventArgs&lt;TEntity&gt;<br/>
        /// EntityCreatedEventArgs&lt;TEntity&gt;<br/>
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<IUpdateModel<TListDTO>> Create(TCreateDTO dto);

        /// <summary>
        /// 批量创建实体。<br/>
        /// 领域事件：<br/>
        /// EntityMappedEventArgs&lt;TEntity, TCreateDTO&gt;<br/>
        /// EntityPreCreateEventArgs&lt;TEntity&gt;<br/>
        /// EntityCreatedEventArgs&lt;TEntity&gt;<br/>
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        Task<IUpdateRangeModel<TListDTO>> CreateRange(TCreateDTO[] dtos);

        /// <summary>
        /// 编辑实体。<br/>
        /// 领域事件：<br/>
        /// EntityPreMapEventArgs&lt;TEntity, TEditDTO&gt;<br/>
        /// EntityMappedEventArgs&lt;TEntity, TEditDTO&gt;<br/>
        /// EntityPreEditEventArgs&lt;TEntity&gt;<br/>
        /// EntityEditedEventArgs&lt;TEntity&gt;<br/>
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<IUpdateModel<TListDTO>> Edit(TEditDTO dto);

        /// <summary>
        /// 批量编辑实体。<br/>
        /// 领域事件：<br/>
        /// EntityPreMapEventArgs&lt;TEntity, TEditDTO&gt;<br/>
        /// EntityMappedEventArgs&lt;TEntity, TEditDTO&gt;<br/>
        /// EntityPreEditEventArgs&lt;TEntity&gt;<br/>
        /// EntityEditedEventArgs&lt;TEntity&gt;<br/>
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        Task<IUpdateRangeModel<TListDTO>> EditRange(TEditDTO[] dtos);

        /// <summary>
        /// 删除实体。<br/>
        /// 领域事件：<br/>
        /// EntityPreRemoveEventArgs&lt;TEntity&gt;<br/>
        /// EntityRemovedEventArgs&lt;TEntity&gt;<br/>
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task Remove(TRemoveDTO dto);

        /// <summary>
        /// 批量删除实体。<br/>
        /// 领域事件：<br/>
        /// EntityPreRemoveEventArgs&lt;TEntity&gt;<br/>
        /// EntityRemovedEventArgs&lt;TEntity&gt;<br/>
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        Task RemoveRange(TRemoveDTO[] dtos);
    }
}
