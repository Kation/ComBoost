using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntityDTO<T> : IEntityDTO
    {
        T Id { get; set; }
    }
}
