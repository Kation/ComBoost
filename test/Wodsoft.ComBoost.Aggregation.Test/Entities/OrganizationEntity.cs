using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Aggregation.Test.Entities
{
    public class OrganizationEntity : EntityBase
    {
        public Guid? ParentId { get; set; }
        private OrganizationEntity _parent;
        public OrganizationEntity Parent { get => _parent; set { _parent = value;ParentId = value?.Id; } }

        [Required]
        [MaxLength(12)]
        public string Name { get; set; }
    }
}
