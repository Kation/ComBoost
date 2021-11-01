using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Aggregation.Test.Entities
{
    public class UserEntity : EntityBase
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(20)]
        public string DisplayName { get; set; }

        public Guid OrganizationId { get; set; }
    }
}
