using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;

namespace Wodsoft.ComBoost.Forum.Entity
{
    public class Post : EntityBase, IPost
    {
        public virtual string Content { get; set; }

        public virtual Member Member { get; set; }

        public virtual Thread Thread { get; set; }

        IMember IPost.Member { get { return Member; } set { Member = (Member)value; } }

        IThread IPost.Thread { get { return Thread; } set { Thread = (Thread)value; } }
    }
}
