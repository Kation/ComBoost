using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public class TestB : EntityBase
    {
        public virtual string Name { get; set; }

        public virtual List<TestA> ACollection { get; set; }
    }
}
