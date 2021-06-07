using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public interface IExecutionResultHandler
    {
        Task Handle(IDomainExecutionContext executionContext, HttpContext httpContext);
    }
}
