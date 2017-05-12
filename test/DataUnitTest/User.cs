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
        private Category _Category;
        [ForeignKey("CategoryId")]
        public virtual Category Category { get { return _Category; }set { _Category = value; CategoryId = value != null ? value.Index : Guid.Empty ; } }

        ICategory IUser.Category { get { return Category; } set { Category = (Category)value; } }
    }
}
