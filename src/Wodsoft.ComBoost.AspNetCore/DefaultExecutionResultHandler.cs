using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class DefaultExecutionResultHandler : IExecutionResultHandler
    {
        public async Task Handle(IDomainExecutionContext executionContext, HttpContext httpContext)
        {
            if (executionContext.Result != null)
            {
                var type = executionContext.DomainMethod.ReturnType.GetGenericArguments()[0];
                httpContext.Response.ContentType = "application/json;charset=utf-8";
                await System.Text.Json.JsonSerializer.SerializeAsync(httpContext.Response.Body, executionContext.Result, type);
            }
        }
    }
}
