using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Forum.Core
{
    public interface IThread : IEntity
    {
        IMember Member { get; set; }

        IForum Forum { get; set; }

        string Title { get; set; }
    }
}
