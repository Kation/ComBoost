using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    public interface ICacheEntity : IEntity
    {
        DateTime UpdateTime { get; set; }
        
        CacheEntityState EntityState { get; set; }
    }
}
