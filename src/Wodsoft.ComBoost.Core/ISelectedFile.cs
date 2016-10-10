using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 选择的文件。
    /// </summary>
    public interface ISelectedFile
    {
        /// <summary>
        /// 获取文件名。
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// 获取文件MIME类型。
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// 获取文件流。
        /// </summary>
        Stream Stream { get; }
    }
}
