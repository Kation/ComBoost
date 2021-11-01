using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Aggregation.Test.Models
{
    public class Organization
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        [Aggregate(typeof(Organization), "Parent")]
        public virtual Guid? ParentId { get; set; }
    }
}
