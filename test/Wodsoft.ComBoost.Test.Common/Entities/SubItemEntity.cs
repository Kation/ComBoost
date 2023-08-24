using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Test.Entities
{
    public class SubItemEntity : EntityBase<Guid>
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public Guid TestId { get; set; }
        private TestEntity _Test;
        public TestEntity Test { get => _Test; set { _Test = value;TestId = value?.Id ?? Guid.Empty; } }

    }
}
