using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 序列化提供器。
    /// </summary>
    public interface ISerializerProvider
    {
        /// <summary>
        /// 获取序列化器。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <returns>返回序列化器。</returns>
        ISerializer GetSerializer(Type type);
    }
}
