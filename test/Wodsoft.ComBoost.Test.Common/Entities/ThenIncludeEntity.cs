using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Test.Entities
{
    public class ThenIncludeEntity : EntityBase<Guid>
    {
        public string Text { get; set; }
    }
}
