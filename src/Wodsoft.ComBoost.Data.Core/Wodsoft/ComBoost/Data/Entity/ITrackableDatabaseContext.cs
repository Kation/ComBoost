using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface ITrackableDatabaseContext : IDatabaseContext
    {
        bool TrackEntity { get; set; }
    }
}
