using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;

namespace Wodsoft.ComBoost.Forum.Entity
{
    [DisplayName("论坛板块")]
    [DisplayColumn("Name", "Name", false)]
    public class Forum : EntityBase, IForum
    {
        [Display(Name = "板块说明", Order = 10)]
        public virtual string Description { get; set; }

        [Display(Name = "板块名称", Order = 0)]
        [Required]
        public virtual string Name { get; set; }
        
        [Hide]
        public virtual ICollection<Thread> Threads { get; set; }
    }
}
