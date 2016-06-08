using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Forum.Core
{
    public interface IForum
    {
        string Name { get; set; }

        string Description { get; set; }
    }
}
