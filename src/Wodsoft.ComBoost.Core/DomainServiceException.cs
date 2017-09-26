using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务预期异常。
    /// </summary>
    public class DomainServiceException : Exception
    {
        /// <summary>
        /// 初始化领域服务器异常。
        /// </summary>
        /// <param name="innerException">内部异常。</param>
        public DomainServiceException(Exception innerException) : this("领域服务预期异常。", innerException)
        { }

        /// <summary>
        /// 初始化领域服务器异常。
        /// </summary>
        /// <param name="message">异常消息内容。</param>
        /// <param name="innerException">内部异常。</param>
        public DomainServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
