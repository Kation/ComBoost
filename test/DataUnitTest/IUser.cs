using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    [EntityInterface]
    public interface IUser : IEntity
    {
        string Username { get; set; }

        ICategory Category { get; set; }
    }
}
