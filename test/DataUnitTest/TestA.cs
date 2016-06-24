using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public class TestA : EntityBase
    {
        public virtual string Name { get; set; }

        public virtual List<TestB> BCollection { get; set; }
    }
}
