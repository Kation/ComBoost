using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Forum.Core
{
    public interface IPost
    {
        IMember Member { get; set; }

        string Content { get; set; }

        DateTime CreateDate { get; set; }
    }
}
