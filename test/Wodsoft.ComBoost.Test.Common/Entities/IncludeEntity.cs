using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Wodsoft.ComBoost.Test.Entities
{
    public class IncludeEntity
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public Guid? ThenIncludeId;
        private ThenIncludeEntity _thenInclude;
        public ThenIncludeEntity ThenInclude { get => _thenInclude; set { _thenInclude = value; ThenIncludeId = value?.Id ?? Guid.Empty; } }
    }
}
