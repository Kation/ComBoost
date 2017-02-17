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
    [DisplayName("论坛")]
    [DisplayColumn("Name", "Order", false)]
    [EntityAuthentication(AllowAnonymous = false)]
    public class Forum : EntityBase, IForum
    {
        [Display(Name = "论坛说明", Order = 20)]
        public virtual string Description { get; set; }

        [Display(Name = "论坛名称", Order = 10)]
        [Required]
        public virtual string Name { get; set; }

        [Display(Name = "所属板块", Order = 0)]
        [Required]
        public virtual Board Board { get; set; }

        [Display(Name = "图标", Order = 30)]
        [CustomDataType(CustomDataType.Image)]
        public virtual string Image { get; set; }

        [Display(Name = "排序", Order = 40)]
        [Required]
        public virtual int Order { get; set; }

        IBoard IForum.Board { get { return Board; } set { Board = (Board)value; } }

        [Hide]
        public virtual ICollection<Thread> Threads { get; set; }
    }
}
