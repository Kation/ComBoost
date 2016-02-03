using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    /// <summary>
    /// Extensions of entity.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Get database context from a context builder.
        /// </summary>
        /// <param name="builder">Entity context builder.</param>
        /// <returns></returns>
        public static DbContext GetContext(IEntityContextBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (builder is EntityContextBuilder)
                return ((EntityContextBuilder)builder).DbContext;
            return null;
        }
    }
}
