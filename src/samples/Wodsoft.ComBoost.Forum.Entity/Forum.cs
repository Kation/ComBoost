using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;

namespace Wodsoft.ComBoost.Forum.Entity
{
    public class Forum : EntityBase, IForum
    {
        public virtual string Description { get; set; }

        public virtual string Name { get; set; }

        public virtual ICollection<Thread> Threads { get; set; }
    }
}
