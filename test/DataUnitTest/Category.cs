using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public class Category : EntityBase
    {
        public virtual string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
