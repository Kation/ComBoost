using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Test.Models
{
    public class ThenIncludeDto : EntityDTOBase<Guid>
    {
        public string Text { get; set; }
    }
}
