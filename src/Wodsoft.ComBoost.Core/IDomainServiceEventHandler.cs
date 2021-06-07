using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainServiceEventHandler<T> where T : DomainServiceEventArgs
    {
        Task Handle(IDomainExecutionContext context, T args);
    }
}
