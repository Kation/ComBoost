using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Aggregation.Test.Models
{
    [Aggregate(typeof(Organization), "Organization", "OrganizationId", IsSelfIgnored = true)]
    public class User : EntityDTOBase<Guid>
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public virtual Guid OrganizationId { get; set; }
    }
}
