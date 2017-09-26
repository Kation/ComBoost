using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public class UserInfo : EntityBase
    {
        [Required]
        public virtual User User { get; set; }

        public virtual string RealName { get; set; }
    }
}
