using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Test.Entities
{
    public class UserEntity: UserBase
    {
        [Key]
        [Required]
        [MaxLength(16)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(16)]
        public string DisplayName { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }
    }
}
