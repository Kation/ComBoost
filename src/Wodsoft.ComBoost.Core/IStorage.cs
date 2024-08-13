using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 存储服务。
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// 保存文件。
        /// </summary>
        /// <param name="stream">要保存的文件流。</param>
        /// <param name="filename">文件名。</param>
        /// <returns>返回保存后的文件路径。</returns>
        Task<string> PutAsync(Stream stream, string filename);

        /// <summary>
        /// 获取文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <returns>返回文件流。</returns>
        Task<Stream?> GetAsync(string path);

        /// <summary>
        /// 获取文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <returns>返回文件。</returns>
        Task<IStorageFile?> GetFileAsync(string path);

        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <returns>成功返回true，失败返回false。</returns>
        Task<bool> DeleteAsync(string path);
    }
}
