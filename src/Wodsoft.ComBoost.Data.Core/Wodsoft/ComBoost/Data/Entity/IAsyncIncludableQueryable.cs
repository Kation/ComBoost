using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IAsyncIncludableQueryable<out TEntity, out TProperty> : IAsyncQueryable<TEntity>
    {
    }
}
