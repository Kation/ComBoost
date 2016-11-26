using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;

namespace Wodsoft.ComBoost.Forum.Entity
{
    public class Post : EntityBase, IPost
    {
        [Required]
        public virtual string Content { get; set; }

        [Required]
        [Hide(IsHiddenOnView = false)]
        public virtual Member Member { get; set; }

        [Required]
        public virtual Thread Thread { get; set; }

        IMember IPost.Member { get { return Member; } set { Member = (Member)value; } }

        IThread IPost.Thread { get { return Thread; } set { Thread = (Thread)value; } }
    }
}
