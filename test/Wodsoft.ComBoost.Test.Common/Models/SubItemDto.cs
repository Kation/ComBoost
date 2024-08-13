using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Test.Models
{
    public class SubItemDto : EntityDTOBase<Guid>
    {
        public string Name { get; set; }

        public Guid TestId { get; set; }
    }
}
