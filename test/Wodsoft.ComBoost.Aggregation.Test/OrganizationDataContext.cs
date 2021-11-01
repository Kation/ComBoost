using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Aggregation.Test.Entities;

namespace Wodsoft.ComBoost.Aggregation.Test
{
    public class OrganizationDataContext : DbContext
    {
        public OrganizationDataContext() { }

        public OrganizationDataContext(DbContextOptions<OrganizationDataContext> options) : base(options) { }

        public DbSet<OrganizationEntity> Organizations { get; set; }
    }
}
