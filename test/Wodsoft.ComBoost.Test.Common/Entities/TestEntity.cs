using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Test.Entities
{
    public class TestEntity : EntityBase
    {
        public int ValueInt { get; set; }

        public long ValueLong { get; set; }

        public float ValueFloat { get; set; }

        public double ValueDouble { get; set; }

        public decimal ValueDecimal { get; set; }

        public Guid? IncludeId;
        private IncludeEntity _include;
        public IncludeEntity Include { get => _include; set { _include = value; IncludeId = value?.Id ?? Guid.Empty; } }

        public ICollection<SubItemEntity> Items { get; set; }
    }
}
