using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public class User : EntityBase
    {
        public virtual string Username { get; set; }

        public virtual Category Category { get; set; }
    }
}
