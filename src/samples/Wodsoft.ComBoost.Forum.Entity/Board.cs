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
    [DisplayName("板块")]
    [DisplayColumn("Name", "Order", false)]
    public class Board : EntityBase, IBoard
    {
        [Display(Name = "板块名称", Order = 0)]
        [Required]
        public virtual string Name { get; set; }

        [Display(Name = "板块说明", Order = 10)]
        [Required]
        public virtual string Description { get; set; }

        [Display(Name = "排序", Order = 20)]
        [Required]
        public virtual int Order { get; set; }

        [Hide]
        public virtual ICollection<Forum> Forums { get; set; }

        ICollection<IForum> IBoard.Forums { get { throw new NotSupportedException(); } }
    }
}
