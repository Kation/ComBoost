using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Test.Models
{
    public class IncludeDto : EntityDTOBase<Guid>
    {
        public string Text { get; set; }

        public Guid? ThenIncludeId;

        public ThenIncludeDto ThenInclude { get; set; }
    }
}
