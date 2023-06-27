using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Test.Models
{
    public class TestDto : EntityDTOBase<Guid>
    {
        public int ValueInt { get; set; }

        public long ValueLong { get; set; }

        public float ValueFloat { get; set; }

        public double ValueDouble { get; set; }

        public decimal ValueDecimal { get; set; }

        public Guid? IncludeId { get; set; }

        public IncludeDto Include { get; set; }

        public ICollection<SubItemDto> Items { get; set; }
    }
}
