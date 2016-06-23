using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;

namespace Wodsoft.ComBoost.Forum.Entity
{
    public class Thread : EntityBase, IThread
    {
        public virtual Forum Forum { get; set; }

        public virtual Member Member { get; set; }

        public virtual string Title { get; set; }
        
        IForum IThread.Forum { get { return Forum; } set { Forum = (Forum)value; } }
        
        IMember IThread.Member { get { return Member; } set { Member = (Member)value; } }
    }
}
