using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity
{
    public class EntityChangeRecord
    {
        public string Type { get; set; }

        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public EntityChangeMode Mode { get; set; }
    }

    public enum EntityChangeMode : byte
    {
        Add = 0,
        Edit = 1,
        Remove = 2
    }
}
