using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Forum
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }
    }
}
