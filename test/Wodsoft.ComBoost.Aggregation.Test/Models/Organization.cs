using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Aggregation.Test.Models
{
    [Aggregate(typeof(Organization), "Parent", "ParentId")]
    public class Organization
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual Guid? ParentId { get; set; }
    }
}
