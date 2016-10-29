using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    [DisplayColumn("Username", "Username", false)]
    public class User : EntityBase, IUser
    {
        public User()
        {

        }

        public virtual string Username { get; set; }

        public virtual Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        ICategory IUser.Category { get { return Category; } set { Category = (Category)value; } }
    }
}
