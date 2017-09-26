using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Forum.Core
{
    [EntityInterface]
    public interface IForum : IEntity
    {
        string Name { get; set; }

        IBoard Board { get; set; }
    }
}
