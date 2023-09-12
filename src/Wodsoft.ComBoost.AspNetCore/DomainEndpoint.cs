using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public abstract class DomainEndpoint
    {
        public abstract Task HandleRequest(HttpContext httpContext);

        public abstract string EndpointTemplate { get; }
    }

    public abstract class DomainEndpoint<T> : DomainEndpoint
        where T : class, IDomainTemplate
    {
        public override Task HandleRequest(HttpContext httpContext)
        {
            var method = httpContext.GetRouteValue("method") as string;
            if (method == null)
            {
                httpContext.Response.StatusCode = 404;
                return Task.CompletedTask;
            }
            var domainTemplate = httpContext.RequestServices.GetRequiredService<T>();
            OnDomainTemplateResolved(domainTemplate);
            return HandleRequest(httpContext, domainTemplate, method);
        }

        protected abstract Task HandleRequest(HttpContext httpContext, T domainTemplate, string method);

        public override string EndpointTemplate
        {
            get
            {
                return "/api/" + GetTemplateName() + "/{method}";
            }
        }

        protected virtual TValue? GetQueryValue<TValue>(HttpContext httpContext, string name)
        {
            if (!httpContext.Request.Query.ContainsKey(name))
                return default;
            var type = typeof(TValue);
            var typeConverter = TypeDescriptor.GetConverter(type);
            if (!typeConverter.CanConvertFrom(typeof(string)))
                return default;
            return (TValue?)typeConverter.ConvertFromString(httpContext.Request.Query[name]);
        }

        protected virtual TValue? GetQueryNullableValue<TValue>(HttpContext httpContext, string name)
            where TValue : struct
        {
            if (!httpContext.Request.Query.ContainsKey(name))
                return null;
            var type = typeof(TValue);
            var typeConverter = TypeDescriptor.GetConverter(type);
            if (!typeConverter.CanConvertFrom(typeof(string)))
                return null;
            return (TValue)typeConverter.ConvertFromString(httpContext.Request.Query[name])!;
        }

        protected virtual TValue[] GetQueryArrayValue<TValue>(HttpContext httpContext, string name)
        {
            if (!httpContext.Request.Query.ContainsKey(name))
                return Array.Empty<TValue>();
            var type = typeof(TValue);
            var typeConverter = TypeDescriptor.GetConverter(type);
            if (!typeConverter.CanConvertFrom(typeof(string)))
                return Array.Empty<TValue>();
            var values = httpContext.Request.Query[name];
            var array = new TValue[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                var value = typeConverter.ConvertFromString(values[i]);
                if (value != null)
                    array[i] = (TValue)value;
            }
            return array;
        }

        protected virtual List<TValue> GetQueryListValue<TValue>(HttpContext httpContext, string name)
        {
            if (!httpContext.Request.Query.ContainsKey(name))
                return new List<TValue>();
            var type = typeof(TValue);
            var typeConverter = TypeDescriptor.GetConverter(type);
            if (!typeConverter.CanConvertFrom(typeof(string)))
                return new List<TValue>();
            var values = httpContext.Request.Query[name];
            var list = new List<TValue>(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                var value = typeConverter.ConvertFromString(values[i]);
                if (value != null)
                    list.Add((TValue)value);
            }
            return list;
        }

        protected virtual async ValueTask<(bool, TValue?)> GetBodyValueAsync<TValue>(HttpContext httpContext)
        {
#if NETCOREAPP2_1 || NETCOREAPP3_1
            if (!MediaTypeHeaderValue.TryParse(httpContext.Request.ContentType, out var mt))
            {
                httpContext.Response.StatusCode = 415;
                return (false, default(TValue));
            }
            if (mt.MediaType != "application/json")
            {
                httpContext.Response.StatusCode = 415;
                return (false, default(TValue));
            }
            return (true, await System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(httpContext.Request.Body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
#else
            if (!httpContext.Request.HasJsonContentType())
            {
                httpContext.Response.StatusCode = 415;
                return (false, default(TValue));
            }
            return (true, await httpContext.Request.ReadFromJsonAsync<TValue>());
#endif
        }

        protected virtual string GetTemplateName()
        {
            var name = GetType().Name.AsSpan();
            if (name.EndsWith("Endpoint", StringComparison.OrdinalIgnoreCase))
                name = name.Slice(0, name.Length - 8);
            return new string(name);
        }

        protected virtual void OnDomainTemplateResolved(T domainTemplate) { }

        private readonly static JsonSerializerOptions _JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        protected virtual Task OnDomainExecuted<TResult>(HttpContext httpContext, string method, TResult value)
        {
            if (value is Stream stream)
            {
                httpContext.Response.ContentType = "application/octet-stream";
                return stream.CopyToAsync(httpContext.Response.Body);
            }
            else
            {
                httpContext.Response.ContentType = "application/json";
                return JsonSerializer.SerializeAsync(httpContext.Response.Body, value, _JsonOptions, httpContext.RequestAborted);
            }
        }

        protected virtual Task OnDomainExecuted(HttpContext httpContext, string method)
        {
            httpContext.Response.StatusCode = 200;
            return Task.CompletedTask;
        }

        protected virtual Task OnDomainException(HttpContext httpContext, string method, Exception exception)
        {
            var logger = httpContext.RequestServices.GetRequiredService<ILogger<DomainEndpoint<T>>>();
            logger.LogError(exception, "Domain execute throw exception.");
            httpContext.Response.StatusCode = 500;
            return Task.CompletedTask;
        }
    }
}
