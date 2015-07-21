using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity.Metadata
{
    /// <summary>
    /// Entity analyzer interface.
    /// </summary>
    public interface IEntityAnalyzer
    {
        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEntityMetadata GetMetadata(Type type);
    }
}
