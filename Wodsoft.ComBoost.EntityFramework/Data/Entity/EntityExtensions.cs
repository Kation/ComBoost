using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    public static class EntityExtensions
    {
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
