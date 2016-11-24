using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;

namespace Wodsoft.ComBoost.Forum.Entity
{
    [DisplayName("板块主题")]
    [DisplayColumn("Name", "CreateDate", true)]
    public class Thread : EntityBase, IThread
    {
        [Display(Name = "所属板块", Order = 0)]
        public virtual Forum Forum { get; set; }

        [Display(Name = "创建用户", Order = 10)]
        public virtual Member Member { get; set; }

        [Display(Name = "标题", Order = 20)]
        public virtual string Title { get; set; }

        [Hide]
        IForum IThread.Forum { get { return Forum; } set { Forum = (Forum)value; } }

        [Hide]
        IMember IThread.Member { get { return Member; } set { Member = (Member)value; } }

        public virtual ICollection<Post> Replies { get; set; }
        
        ICollection<IPost> IThread.Replies { get { throw new NotSupportedException(); } }
    }
}
