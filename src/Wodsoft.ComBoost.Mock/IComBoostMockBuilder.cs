using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public interface IComBoostMockBuilder
    {
        IServiceCollection Services { get; }
    }
}
