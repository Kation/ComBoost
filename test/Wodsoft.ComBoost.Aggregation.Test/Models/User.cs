using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Aggregation.Test.Models
{
    public class User : EntityDTOBase<Guid>
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        [Aggregate(typeof(Organization), "Organization", IsSelfIgnored = true)]
        public virtual Guid OrganizationId { get; set; }
    }
}
