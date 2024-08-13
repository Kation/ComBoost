using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore.Test.Endpoints
{
    public abstract class ApiEndpoint<T> : DomainEndpoint<T>
        where T : class, IDomainTemplate
    {
        private readonly static JsonSerializerOptions _JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        protected override Task OnDomainExecuted(HttpContext httpContext, string method)
        {
            httpContext.Response.ContentType = "application/json";
            return JsonSerializer.SerializeAsync(httpContext.Response.Body, new ApiResult
            {
                Code = 0,
                Message = "Success",
            }, _JsonOptions, httpContext.RequestAborted);
        }

        protected override Task OnDomainExecuted<TResult>(HttpContext httpContext, string method, TResult value)
        {
            httpContext.Response.ContentType = "application/json";
            return JsonSerializer.SerializeAsync(httpContext.Response.Body, new ApiResult<TResult>
            {
                Code = 0,
                Message = "Success",
                Content = value
            }, _JsonOptions, httpContext.RequestAborted);
        }

        protected override Task OnDomainException(HttpContext httpContext, string method, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";
            return JsonSerializer.SerializeAsync(httpContext.Response.Body, new ApiResult
            {
                Code = 500,
                Message = exception.Message,
            }, _JsonOptions, httpContext.RequestAborted);
        }
    }
}
