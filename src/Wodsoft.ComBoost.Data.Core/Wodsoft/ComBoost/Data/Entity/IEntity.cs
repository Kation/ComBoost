﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 实体定义接口。
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 创建时。
        /// 通常于创建实体实例时调用。
        /// </summary>
        void OnCreating();

        /// <summary>
        /// 创建完毕。
        /// 通常于向数据库插入之前调用。
        /// </summary>
        void OnCreateCompleted();

        /// <summary>
        /// 编辑时。
        /// 通常于在用户编辑之前调用。
        /// </summary>
        void OnEditing();

        /// <summary>
        /// 编辑完毕时。
        /// 通常于向数据库更新之前调用。
        /// </summary>
        void OnEditCompleted();

        /// <summary>
        /// 获取是否允许删除。
        /// </summary>
        bool IsRemoveAllowed { get; }

        /// <summary>
        /// 获取是否允许编辑。
        /// </summary>
        bool IsEditAllowed { get; }

        /// <summary>
        /// 获取是否是新创建的实体。
        /// </summary>
        [Obsolete("No used anymore.")]
        bool IsNewCreated { get; }
    }
}
