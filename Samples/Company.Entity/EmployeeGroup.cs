using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Entity
{
    [DisplayName("员工组")]
    [DisplayColumn("Name")]
    [EntityAuthentication(AllowAnonymous = false, ViewRolesRequired = new string[] { "Admin" })]
    public class EmployeeGroup : EntityBase
    {
        [Display(Name = "员工组名称", Order = 0)]
        [Required]
        public virtual string Name { get; set; }

        [Display(Name = "员工组权限", Order = 10)]
        [Required]
        public virtual EmployeePower Power { get; set; }

        [Hide]
        public virtual ICollection<Employee> Employees { get; set; }
    }

    public enum EmployeePower
    {
        Admin = 0,
        Normal = 1
    }
}
