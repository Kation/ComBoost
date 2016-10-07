using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.Forum.Core
{
    public interface IMember : IEntity, IPermission
    {
        string Username { get; set; }

        void SetPassword(string password);

        bool VerifyPassword(string password);

        ICollection<IThread> Threads { get; }
    }
}
