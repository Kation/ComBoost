using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Forum.Core
{
    public interface IThread
    {
        IMember Member { get; set; }

        string Title { get; set; }

        DateTime CreateDate { get; set; }
    }
}
