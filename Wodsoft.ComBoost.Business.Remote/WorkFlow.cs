using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost.Business
{
    public class WorkFlow
    {
        public string Name { get; set; }

        public string[] Roles { get; set; }
        
        public WorkItem[] Items { get; set; }

        public string Description { get; set; }
        
        public int Order { get; set; }
    }
}
