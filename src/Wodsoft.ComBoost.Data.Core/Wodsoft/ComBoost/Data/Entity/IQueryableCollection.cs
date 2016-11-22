using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IQueryableCollection<T> : ICollection<T>, IQueryable<T>
    {
    }
}
