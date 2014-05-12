using Company.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Wpf
{
    public class DataContext : DbContext
    {
        public DbSet<Employee> Employee { get; set; }

        public DbSet<EmployeeGroup> EmployeeGroup { get; set; }
    }
}
