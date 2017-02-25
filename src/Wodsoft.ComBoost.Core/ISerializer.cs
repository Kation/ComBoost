using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 序列化器。
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="stream">保存至的流。</param>
        /// <param name="value">要序列化的值。</param>
        void Serialize(Stream stream, object value);

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="stream">包含序列化后内容的流。</param>
        /// <returns>返回值。</returns>
        object Deserialize(Stream stream);
    }
}
