using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public class Category : EntityBase, ICategory
    {
        public virtual string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
        
        ICollection<IUser> ICategory.Users
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}
