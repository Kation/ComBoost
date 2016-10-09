using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public interface ICategory : IEntity
    {
        string Name { get; set; }

        ICollection<IUser> Users { get; }
    }
}
