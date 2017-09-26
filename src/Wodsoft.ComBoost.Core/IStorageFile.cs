using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 存储文件。
    /// </summary>
    public interface IStorageFile
    {
        /// <summary>
        /// 获取文件流。
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// 获取文件路径。
        /// </summary>
        string Path { get; }

        /// <summary>
        /// 获取修改日期。
        /// </summary>
        DateTime ModifiedDate { get; }
    }
}
