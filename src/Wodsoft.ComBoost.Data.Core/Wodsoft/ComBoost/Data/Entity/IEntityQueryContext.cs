using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 实体查询上下文。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public interface IEntityQueryContext<out T>
        where T : IEntity
    {
        /// <summary>
        /// 获取相关数据库上下文。
        /// </summary>
        IDatabaseContext Database { get; }

        /// <summary>
        /// 获取实体元数据。
        /// </summary>
        IEntityMetadata Metadata { get; }

        /// <summary>
        /// 获取查询接口。
        /// </summary>
        /// <returns>返回查询对象。</returns>
        IQueryable<T> Query();        
    }
}
